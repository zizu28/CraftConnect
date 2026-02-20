using BookingManagement.Application.SAGA;
using BookingManagement.Infrastructure.SAGAS;
using Core.SharedKernel.Commands.BookingCommands;
using Core.SharedKernel.Commands.NotificationCommands;
using Core.SharedKernel.Commands.PaymentCommands;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CraftConnect.Tests.SAGAS
{
	/// <summary>
	/// Tests for the BookingToPaymentSaga distributed transaction state machine.
	/// Uses MassTransit InMemory TestHarness (no external dependencies needed).
	///
	/// MassTransit 9.x API notes:
	///   - sagaHarness.Exists(Guid correlationId)    → polls until saga exists
	///   - sagaHarness.NotExists(Guid correlationId) → polls until saga is gone
	///   - sagaHarness.Sagas                          → enumerate for predicate finds
	/// </summary>
	public class BookingToPaymentSagaTests : IAsyncLifetime
	{
		private ServiceProvider _serviceProvider = null!;
		private ITestHarness _harness = null!;
		private ISagaStateMachineTestHarness<BookingToPaymentSaga, BookingToPaymentState> _sagaHarness = null!;

		// ── Reusable test data ─────────────────────────────────────────────
		private static readonly Guid BookingId = Guid.NewGuid();
		private static readonly Guid CraftspersonId = Guid.NewGuid();
		private static readonly Guid CustomerId = Guid.NewGuid();
		private static readonly Guid PaymentId = Guid.NewGuid();
		private static readonly Money TestAmount = new(150.00m, "USD");

		// ── State name constants ───────────────────────────────────────────
		private const string State_WaitingForPaymentInitiation = "WaitingForPaymentInitiation";
		private const string State_WaitingForPaymentCompletion = "WaitingForPaymentCompletion";
		private const string State_WaitingForBookingConfirmation = "WaitingForBookingConfirmation";
		private const string State_WaitingForNotification = "WaitingForNotification";
		private const string State_CompensatingBooking = "CompensatingBooking";
		private const string State_CompensatingPayment = "CompensatingPayment";

		public async Task InitializeAsync()
		{
			_serviceProvider = new ServiceCollection()
				.AddMassTransitTestHarness(cfg =>
				{
					cfg.AddSagaStateMachine<BookingToPaymentSaga, BookingToPaymentState>();
				})
				.BuildServiceProvider(true);

			_harness = _serviceProvider.GetRequiredService<ITestHarness>();
			_sagaHarness = _harness.GetSagaStateMachineHarness<BookingToPaymentSaga, BookingToPaymentState>();
			await _harness.Start();
		}

		public async Task DisposeAsync()
		{
			await _harness.Stop();
			await _serviceProvider.DisposeAsync();
		}

		// ─────────────────────────────────────────────────────────────────
		// Happy Path
		// ─────────────────────────────────────────────────────────────────

		#region Happy Path

		[Fact]
		public async Task BookingRequested_ShouldCreateSagaInstance_InWaitingForPaymentInitiation()
		{
			// Act
			await _harness.Bus.Publish(BuildBookingRequested(BookingId));

			// Assert – SAGA created and moved to correct state
			var saga = await PollForSagaAsync(s => s.BookingId == BookingId);
			Assert.NotNull(saga);
			Assert.Equal(State_WaitingForPaymentInitiation, saga.CurrentState);

			// Confirm via the harness Exists helper (non-null return = saga exists)
			Assert.NotNull(await _sagaHarness.Exists(saga.CorrelationId));
		}

		[Fact]
		public async Task BookingRequested_ShouldPublish_InitiatePaymentCommand()
		{
			// Act
			await _harness.Bus.Publish(BuildBookingRequested(BookingId));

			// Assert
			Assert.True(await _harness.Published.Any<InitiatePaymentCommand>());
		}

		[Fact]
		public async Task BookingRequested_ShouldStoreCorrectSagaData()
		{
			// Act
			await _harness.Bus.Publish(BuildBookingRequested(BookingId));

			// Assert
			var saga = await PollForSagaAsync(s => s.BookingId == BookingId);
			Assert.NotNull(saga);
			Assert.Equal(BookingId, saga.BookingId);
			Assert.Equal(CraftspersonId, saga.CraftsmanId);
			Assert.Equal("Fix leaking pipe", saga.ServiceDescription);
		}

		[Fact]
		public async Task PaymentInitiated_ShouldTransitionTo_WaitingForPaymentCompletion()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentInitiation(BookingId);

			// Act
			await _harness.Bus.Publish(BuildPaymentInitiated(PaymentId, BookingId));

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_WaitingForPaymentCompletion);
			Assert.NotNull(saga);
		}

		[Fact]
		public async Task PaymentInitiated_ShouldStorePaymentId_InSagaState()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentInitiation(BookingId);

			// Act
			await _harness.Bus.Publish(BuildPaymentInitiated(PaymentId, BookingId));

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId && s.PaymentId == PaymentId);
			Assert.NotNull(saga);
			Assert.Equal(PaymentId, saga.PaymentId);
			Assert.NotNull(saga.PaymentInitiatedAt);
		}

		[Fact]
		public async Task PaymentCompleted_ShouldTransitionTo_WaitingForBookingConfirmation()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentCompletion(BookingId, PaymentId);

			// Act
			await _harness.Bus.Publish(BuildPaymentCompleted(PaymentId, BookingId));

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_WaitingForBookingConfirmation);
			Assert.NotNull(saga);
		}

		[Fact]
		public async Task PaymentCompleted_ShouldPublish_ConfirmBookingCommand_WithCorrectIds()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentCompletion(BookingId, PaymentId);

			// Act
			await _harness.Bus.Publish(BuildPaymentCompleted(PaymentId, BookingId));

			// Assert – command published
			Assert.True(await _harness.Published.Any<ConfirmBookingCommand>());

			var cmd = _harness.Published.Select<ConfirmBookingCommand>().First().Context.Message;
			Assert.Equal(BookingId, cmd.BookingId);
			Assert.Equal(PaymentId, cmd.PaymentId);
		}

		[Fact]
		public async Task BookingConfirmed_ShouldTransitionTo_WaitingForNotification()
		{
			// Arrange
			await PublishUpTo_WaitingForBookingConfirmation(BookingId, PaymentId);

			// Act
			await _harness.Bus.Publish(BuildBookingConfirmed(BookingId));

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_WaitingForNotification);
			Assert.NotNull(saga);
		}

		[Fact]
		public async Task BookingConfirmed_ShouldPublish_ConfirmationNotificationCommand()
		{
			// Arrange
			await PublishUpTo_WaitingForBookingConfirmation(BookingId, PaymentId);

			// Act
			await _harness.Bus.Publish(BuildBookingConfirmed(BookingId));

			// Assert
			Assert.True(await _harness.Published.Any<SendBookingConfirmationNotificationCommand>());

			var cmd = _harness.Published
				.Select<SendBookingConfirmationNotificationCommand>()
				.First().Context.Message;
			Assert.Equal(BookingId, cmd.BookingId);
			Assert.Equal("Fix leaking pipe", cmd.ServiceDescription);
		}

		[Fact]
		public async Task FullHappyPath_ShouldPublishAllCommandsInOrder()
		{
			// Act – simulate full happy path
			await _harness.Bus.Publish(BuildBookingRequested(BookingId));
			await Task.Delay(50);
			await _harness.Bus.Publish(BuildPaymentInitiated(PaymentId, BookingId));
			await Task.Delay(50);
			await _harness.Bus.Publish(BuildPaymentCompleted(PaymentId, BookingId));
			await Task.Delay(50);
			await _harness.Bus.Publish(BuildBookingConfirmed(BookingId));

			// Assert – all 3 inter-module commands published
			Assert.True(await _harness.Published.Any<InitiatePaymentCommand>(),
				"Should publish InitiatePaymentCommand");
			Assert.True(await _harness.Published.Any<ConfirmBookingCommand>(),
				"Should publish ConfirmBookingCommand");
			Assert.True(await _harness.Published.Any<SendBookingConfirmationNotificationCommand>(),
				"Should publish NotificationCommand");

			// Assert – final state
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_WaitingForNotification);
			Assert.NotNull(saga);
		}

		#endregion

		// ─────────────────────────────────────────────────────────────────
		// Payment Failure
		// ─────────────────────────────────────────────────────────────────

		#region Payment Failure

		[Fact]
		public async Task PaymentFailed_ShouldTransitionTo_CompensatingBooking()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentCompletion(BookingId, PaymentId);

			// Act
			await _harness.Bus.Publish(BuildPaymentFailed(PaymentId, BookingId, "Insufficient funds"));

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_CompensatingBooking);
			Assert.NotNull(saga);
		}

		[Fact]
		public async Task PaymentFailed_ShouldStoreFailureReason()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentCompletion(BookingId, PaymentId);

			// Act
			await _harness.Bus.Publish(BuildPaymentFailed(PaymentId, BookingId, "Card declined"));

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId && s.FailureReason != null);
			Assert.NotNull(saga);
			Assert.Contains("Card declined", saga.FailureReason, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task BookingCancelled_WhenCompensatingBooking_ShouldFinalizeSaga()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentCompletion(BookingId, PaymentId);
			await _harness.Bus.Publish(BuildPaymentFailed(PaymentId, BookingId, "Card declined"));
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId && s.CurrentState == State_CompensatingBooking);
			Assert.NotNull(saga);

			// Act
			await _harness.Bus.Publish(BuildBookingCancelled(BookingId));

			// Assert – SAGA finalized (NotExists returns null when the SAGA is gone)
			var notExistsResult = await _sagaHarness.NotExists(saga.CorrelationId);
			Assert.Null(notExistsResult); // null = saga was removed, i.e. finalized
		}

		#endregion

		// ─────────────────────────────────────────────────────────────────
		// Payment Timeout
		// ─────────────────────────────────────────────────────────────────

		#region Payment Timeout

		[Fact]
		public async Task PaymentTimeout_ShouldTransitionTo_CompensatingBooking()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentCompletion(BookingId, PaymentId);
			var correlationId = await GetCorrelationIdAsync(BookingId);

			// Act – simulate the scheduled timeout message
			await _harness.Bus.Publish<PaymentTimeoutExpired>(new
			{
				CorrelationId = correlationId,
				BookingId,
				PaymentId
			});

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_CompensatingBooking);
			Assert.NotNull(saga);
		}

		[Fact]
		public async Task PaymentTimeout_ShouldSetTimeoutFailureReason()
		{
			// Arrange
			await PublishUpTo_WaitingForPaymentCompletion(BookingId, PaymentId);
			var correlationId = await GetCorrelationIdAsync(BookingId);

			// Act
			await _harness.Bus.Publish<PaymentTimeoutExpired>(new
			{
				CorrelationId = correlationId,
				BookingId,
				PaymentId
			});

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId && s.FailureReason != null);
			Assert.NotNull(saga);
			Assert.Contains("timeout", saga.FailureReason, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task PaymentCompleted_AfterTimeout_ShouldNotPublish_ConfirmBookingCommand()
		{
			// Arrange – time out the payment first
			await PublishUpTo_WaitingForPaymentCompletion(BookingId, PaymentId);
			var correlationId = await GetCorrelationIdAsync(BookingId);
			await _harness.Bus.Publish<PaymentTimeoutExpired>(new
			{
				CorrelationId = correlationId,
				BookingId,
				PaymentId
			});
			// Ensure SAGA is in CompensatingBooking before sending late event
			await PollForSagaAsync(s =>
				s.BookingId == BookingId && s.CurrentState == State_CompensatingBooking);

			// Act – late payment completion arrives
			await _harness.Bus.Publish(BuildPaymentCompleted(PaymentId, BookingId));

			// Assert – no ConfirmBookingCommand published (SAGA in wrong state for it)
			Assert.False(await _harness.Published.Any<ConfirmBookingCommand>(),
				"ConfirmBookingCommand should not be published after payment timeout");
		}

		#endregion

		// ─────────────────────────────────────────────────────────────────
		// Booking Confirmation Timeout
		// ─────────────────────────────────────────────────────────────────

		#region Booking Confirmation Timeout

		[Fact]
		public async Task BookingConfirmationTimeout_ShouldTransitionTo_CompensatingPayment()
		{
			// Arrange
			await PublishUpTo_WaitingForBookingConfirmation(BookingId, PaymentId);
			var correlationId = await GetCorrelationIdAsync(BookingId);

			// Act
			await _harness.Bus.Publish<BookingConfirmationTimeoutExpired>(new
			{
				CorrelationId = correlationId,
				BookingId
			});

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_CompensatingPayment);
			Assert.NotNull(saga);
		}

		[Fact]
		public async Task BookingConfirmationTimeout_ShouldIncrementRetryCount_AndSetFailureReason()
		{
			// Arrange
			await PublishUpTo_WaitingForBookingConfirmation(BookingId, PaymentId);
			var correlationId = await GetCorrelationIdAsync(BookingId);

			// Act
			await _harness.Bus.Publish<BookingConfirmationTimeoutExpired>(new
			{
				CorrelationId = correlationId,
				BookingId
			});

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_CompensatingPayment);
			Assert.NotNull(saga);
			Assert.Equal(1, saga.BookingConfirmationRetryCount);
			Assert.Equal("Booking confirmation timeout", saga.FailureReason);
		}

		[Fact]
		public async Task CompensatingPayment_OnPaymentFailed_ShouldTransitionTo_CompensatingBooking()
		{
			// Arrange – reach CompensatingPayment
			await PublishUpTo_WaitingForBookingConfirmation(BookingId, PaymentId);
			var correlationId = await GetCorrelationIdAsync(BookingId);
			await _harness.Bus.Publish<BookingConfirmationTimeoutExpired>(new
			{
				CorrelationId = correlationId,
				BookingId
			});
			await PollForSagaAsync(s =>
				s.BookingId == BookingId && s.CurrentState == State_CompensatingPayment);

			// Act – refund fires PaymentFailed in CompensatingPayment state
			await _harness.Bus.Publish(BuildPaymentFailed(PaymentId, BookingId, "Refund processed"));

			// Assert
			var saga = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_CompensatingBooking);
			Assert.NotNull(saga);
		}

		#endregion

		// ─────────────────────────────────────────────────────────────────
		// Correlation & Isolation
		// ─────────────────────────────────────────────────────────────────

		#region Correlation & Isolation

		[Fact]
		public async Task TwoBookings_ShouldCreateTwoIndependentSagaInstances()
		{
			var booking2Id = Guid.NewGuid();

			// Act
			await _harness.Bus.Publish(BuildBookingRequested(BookingId));
			await _harness.Bus.Publish(BuildBookingRequested(booking2Id));

			// Assert – both SAGAs exist independently
			var saga1 = await PollForSagaAsync(s => s.BookingId == BookingId);
			var saga2 = await PollForSagaAsync(s => s.BookingId == booking2Id);
			Assert.NotNull(saga1);
			Assert.NotNull(saga2);
			Assert.NotEqual(saga1.CorrelationId, saga2.CorrelationId);
		}

		[Fact]
		public async Task PaymentCompletion_ShouldCorrelate_ToCorrectSagaOnly()
		{
			var booking2Id = Guid.NewGuid();
			var payment2Id = Guid.NewGuid();

			// Arrange – two bookings waiting for payment
			await _harness.Bus.Publish(BuildBookingRequested(BookingId));
			await _harness.Bus.Publish(BuildBookingRequested(booking2Id));
			await Task.Delay(50);
			await _harness.Bus.Publish(BuildPaymentInitiated(PaymentId, BookingId));
			await _harness.Bus.Publish(BuildPaymentInitiated(payment2Id, booking2Id));
			await Task.Delay(50);

			// Act – only complete payment for booking 1
			await _harness.Bus.Publish(BuildPaymentCompleted(PaymentId, BookingId));

			// Assert – booking 1 advances, booking 2 stays
			var saga1 = await PollForSagaAsync(s =>
				s.BookingId == BookingId &&
				s.CurrentState == State_WaitingForBookingConfirmation);
			Assert.NotNull(saga1);

			var saga2 = await PollForSagaAsync(s =>
				s.BookingId == booking2Id &&
				s.CurrentState == State_WaitingForPaymentCompletion);
			Assert.NotNull(saga2);
		}

		#endregion

		// ─────────────────────────────────────────────────────────────────
		// Helpers
		// ─────────────────────────────────────────────────────────────────

		#region Helpers

		/// <summary>Advance SAGA to WaitingForPaymentInitiation.</summary>
		private async Task PublishUpTo_WaitingForPaymentInitiation(Guid bookingId)
		{
			await _harness.Bus.Publish(BuildBookingRequested(bookingId));
			await Task.Delay(50);
		}

		/// <summary>Advance SAGA to WaitingForPaymentCompletion.</summary>
		private async Task PublishUpTo_WaitingForPaymentCompletion(Guid bookingId, Guid paymentId)
		{
			await PublishUpTo_WaitingForPaymentInitiation(bookingId);
			await _harness.Bus.Publish(BuildPaymentInitiated(paymentId, bookingId));
			await Task.Delay(50);
		}

		/// <summary>Advance SAGA to WaitingForBookingConfirmation.</summary>
		private async Task PublishUpTo_WaitingForBookingConfirmation(Guid bookingId, Guid paymentId)
		{
			await PublishUpTo_WaitingForPaymentCompletion(bookingId, paymentId);
			await _harness.Bus.Publish(BuildPaymentCompleted(paymentId, bookingId));
			await Task.Delay(50);
		}

		/// <summary>
		/// Poll the in-memory SAGA store for an instance matching the predicate.
		/// Retries for up to <paramref name="timeoutMs"/> milliseconds.
		/// </summary>
		private async Task<BookingToPaymentState?> PollForSagaAsync(
			Func<BookingToPaymentState, bool> predicate,
			int timeoutMs = 3000)
		{
			var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
			while (DateTime.UtcNow < deadline)
			{
				// ISagaList<T> implements IEnumerable - cast to use LINQ
				var match = ((IEnumerable<ISagaInstance<BookingToPaymentState>>)_sagaHarness.Sagas)
					.FirstOrDefault(s => predicate(s.Saga));
				if (match != null)
					return match.Saga;
				await Task.Delay(20);
			}
			return null;
		}

		/// <summary>Get the CorrelationId for a saga identified by BookingId.</summary>
		private async Task<Guid> GetCorrelationIdAsync(Guid bookingId)
		{
			var saga = await PollForSagaAsync(s => s.BookingId == bookingId);
			Assert.NotNull(saga);
			return saga.CorrelationId;
		}

		// ── Event Builders ─────────────────────────────────────────────

		private static BookingRequestedIntegrationEvent BuildBookingRequested(Guid bookingId) =>
			new(bookingId, CraftspersonId,
				new Address("123 Main St", "Lagos", "NG-100"), // 3-arg: street, city, postalCode
				"Fix leaking pipe");

		private static PaymentInitiatedIntegrationEvent BuildPaymentInitiated(Guid paymentId, Guid bookingId) =>
			new(paymentId, null, TestAmount, CustomerId, null);

		private static PaymentCompletedIntegrationEvent BuildPaymentCompleted(Guid paymentId, Guid bookingId) =>
			new(paymentId, bookingId, null, null, TestAmount, CustomerId, null);

		private static PaymentFailedIntegrationEvent BuildPaymentFailed(Guid paymentId, Guid bookingId, string reason) =>
			new(paymentId, bookingId, null, null, reason, CustomerId, null);

		private static BookingConfirmedIntegrationEvent BuildBookingConfirmed(Guid bookingId) =>
			new(bookingId, CustomerId, CraftspersonId, TestAmount.Amount, DateTime.UtcNow);

		private static BookingCancelledIntegrationEvent BuildBookingCancelled(Guid bookingId) =>
			new(bookingId, "Payment failed"); // string? ReasonForCancellation

		#endregion
	}
}
