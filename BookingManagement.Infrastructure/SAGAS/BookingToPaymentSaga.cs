using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.SAGA;
using Core.SharedKernel.Commands.NotificationCommands;
using Core.SharedKernel.Commands.PaymentCommands;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using MassTransit;

namespace BookingManagement.Infrastructure.SAGAS
{
	public class BookingToPaymentSaga : MassTransitStateMachine<BookingToPaymentState>
	{
		public BookingToPaymentSaga()
		{
			InstanceState(x => x.CurrentState);

			Event(() => BookingRequested, x => x.CorrelateBy((state, ctx) => state.BookingId == ctx.Message.BookingId));
			Event(() => PaymentInitiated, x => x.CorrelateBy((state, ctx) => state.PaymentId == ctx.Message.PaymentId));
			Event(() => PaymentCompleted, x => x.CorrelateBy((state, ctx) => state.PaymentId == ctx.Message.PaymentId || state.BookingId == ctx.Message.BookingId));
			Event(() => PaymentFailed, x => x.CorrelateBy((state, ctx) => state.PaymentId == ctx.Message.PaymentId || state.BookingId == ctx.Message.BookingId));
			Event(() => BookingConfirmed, x => x.CorrelateBy((state, ctx) => state.BookingId == ctx.Message.BookingId));
			Event(() => BookingCancelled, x => x.CorrelateBy((state, ctx) => state.BookingId == ctx.Message.BookingId));
			//Event(() => NotificationSent, x => x.CorrelateBy((state, ctx) => state.Notifi == ctx.Message.BookingId));
			Schedule(() => PaymentTimeout, m => m.PaymentTimeoutTokenId, s =>
			{
				s.Delay = TimeSpan.FromMinutes(15);
				s.Received = r => r.CorrelateById(ctx => ctx.Message.CorrelationId);
			});
			Schedule(() => BookingConfirmationTimeout, m => m.BookingConfirmationTimeoutTokenId, s =>
			{
				s.Delay = TimeSpan.FromSeconds(30);
				s.Received = r => r.CorrelateById(context => context.Message.CorrelationId);
			});

			Initially(
				When(BookingRequested)
				.Then(context =>
				{
					context.Saga.BookingId = context.Message.BookingId;
					context.Saga.CraftsmanId = context.Message.CraftspersonId;
					context.Saga.ServiceDescription = context.Message.Description;
					context.Saga.CreatedAt = DateTime.UtcNow;
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.Publish(ctx => ctx.Init<InitiatePaymentCommand>(new
				{
					ctx.Saga.CorrelationId,
					ctx.Saga.BookingId,
					ctx.Saga.Amount,
					ctx.Saga.Currency,
					ctx.Saga.CustomerId
				}))
				.TransitionTo(WaitingForPaymentInitiation)
			);

			During(WaitingForPaymentInitiation,
				When(PaymentInitiated)
				.Then(context =>
				{
					context.Saga.PaymentId = context.Message.PaymentId;
					context.Saga.Amount = context.Message.Amount.Amount;
					context.Saga.Currency = context.Message.Amount.Currency;
					context.Saga.PaymentInitiatedAt = DateTime.UtcNow;
					context.Saga.UpdatedAt = DateTime.UtcNow;

					if (context.Message.PayerId.HasValue)
						context.Saga.CustomerId = context.Message.PayerId.Value;
					if (context.Message.RecipientId.HasValue)
						context.Saga.CraftsmanId = context.Message.RecipientId.Value;
				})
				.Schedule(PaymentTimeout, context => context.Init<PaymentTimeoutExpired>(new
				{
					context.Saga.CorrelationId,
					context.Saga.BookingId,
					context.Saga.PaymentId
				}))
				.TransitionTo(WaitingForPaymentCompletion)
			);

			During(WaitingForPaymentCompletion,
				When(PaymentCompleted)
				.Then(context =>
				{
					context.Saga.PaymentCompletedAt = DateTime.UtcNow;
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.Unschedule(PaymentTimeout)
				.PublishAsync(context => context.Init<ConfirmBookingCommand>(new
				{
					context.Saga.CorrelationId,
					context.Saga.BookingId,
					context.Saga.PaymentId,
				}))
				.TransitionTo(WaitingForBookingConfirmation),

				When(PaymentFailed)
				.Then(context =>
				{
					context.Saga.FailureReason = $"Payment failed: {context.Message.Reason}";
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.Unschedule(PaymentTimeout)
				.TransitionTo(CompensatingBooking),

				When(PaymentTimeout!.Received)
					.Then(context =>
					{
						context.Saga.FailureReason = "Payment timeout - 15 minutes expired";
						context.Saga.UpdatedAt = DateTime.UtcNow;
					})
					.TransitionTo(CompensatingBooking)
			);

			During(WaitingForBookingConfirmation,
				When(BookingConfirmed)
				.Then(context =>
				{
					context.Saga.BookingConfirmedAt = DateTime.UtcNow;
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.Unschedule(BookingConfirmationTimeout)
				.PublishAsync(context => context.Init<SendBookingConfirmationNotificationCommand>(new
				{
					context.Saga.CorrelationId,
					context.Saga.BookingId,
					context.Saga.CustomerEmail,
					context.Saga.ServiceDescription,
					context.Saga.Amount,
					context.Saga.Currency,
					context.Saga.PaymentReference
				}))
				.TransitionTo(WaitingForNotification),
				
				When(BookingConfirmationTimeout!.Received)
				.Then(context =>
				{
					context.Saga.BookingConfirmationRetryCount++;
					context.Saga.FailureReason = "Booking confirmation timeout";
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.TransitionTo(CompensatingPayment)
			);

			During(CompensatingBooking,
				When(BookingCancelled)
				.Then(context =>
				{
					context.Saga.CancelledAt = DateTime.UtcNow;
					context.Saga.CompensationCompleted = true;
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.Finalize()
			);

			During(CompensatingPayment,
				When(PaymentFailed)
				.Then(context =>
				{
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.TransitionTo(CompensatingBooking)
			);

			SetCompletedWhenFinalized();
		}

		public State WaitingForPaymentInitiation { get; private set; }
		public State WaitingForPaymentCompletion { get; private set; }
		public State WaitingForBookingConfirmation { get; private set; }
		public State WaitingForNotification { get; private set; }
		public State CompensatingBooking { get; private set; }
		public State CompensatingPayment { get; private set; }

		public Event<BookingRequestedIntegrationEvent> BookingRequested { get; private set; }
		public Event<PaymentInitiatedIntegrationEvent> PaymentInitiated { get; private set; }
		public Event<PaymentCompletedIntegrationEvent> PaymentCompleted { get; private set; }
		public Event<PaymentFailedIntegrationEvent> PaymentFailed { get; private set; }
		public Event<BookingConfirmedIntegrationEvent> BookingConfirmed { get; private set; }
		public Event<BookingCancelledIntegrationEvent> BookingCancelled { get; private set; }
		public Event<NotificationSentIntegrationEvent> NotificationSent { get; private set; }

		public Schedule<BookingToPaymentState, PaymentTimeoutExpired> PaymentTimeout { get; private set; }
		public Schedule<BookingToPaymentState, BookingConfirmationTimeoutExpired> BookingConfirmationTimeout { get; private set; }
	}
}
