using BookingManagement.Application.SAGA;
using Core.SharedKernel.Commands.BookingCommands;
using Core.SharedKernel.Commands.NotificationCommands;
using Core.SharedKernel.Commands.PaymentCommands;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Core.SharedKernel.IntegrationEvents.RefundIntegrationEvents;
using MassTransit;

namespace BookingManagement.Infrastructure.SAGAS
{
	public class BookingToPaymentSaga : MassTransitStateMachine<BookingToPaymentState>
	{
		public BookingToPaymentSaga()
		{
			InstanceState(x => x.CurrentState);

			Event(() => BookingRequested, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
			Event(() => PaymentInitiated, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
			Event(() => PaymentCompleted, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
			Event(() => PaymentFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
			Event(() => PaymentRefunded, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
			Event(() => BookingConfirmed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
			Event(() => BookingCancelled, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
			Event(() => NotificationSent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
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
					context.Saga.CustomerId = context.Message.CustomerId;
					context.Saga.Amount = context.Message.Amount;
					context.Saga.Currency = context.Message.Currency;
					context.Saga.CustomerEmail = context.Message.CustomerEmail;
					context.Saga.ServiceDescription = context.Message.Description;
					context.Saga.CreatedAt = DateTime.UtcNow;
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.PublishAsync(ctx => ctx.Init<InitiatePaymentCommand>(new
				{
					ctx.Saga.CorrelationId,
					ctx.Saga.BookingId,
					RecipientId = ctx.Saga.CraftsmanId,
					ctx.Saga.CustomerId,
					ctx.Saga.Amount,
					ctx.Saga.Currency,
					CustomerEmail = ctx.Saga.CustomerEmail ?? string.Empty,
					Description = ctx.Saga.ServiceDescription ?? string.Empty,
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
				}), context => TimeSpan.FromMinutes(15))
				.TransitionTo(WaitingForPaymentCompletion),
				
				When(PaymentFailed)
				.Then(context =>
				{
					context.Saga.FailureReason = context.Message.Reason;
				})
				.PublishAsync(ctx => ctx.Init<CancelBookingCommand>(new
				{
					ctx.Saga.CorrelationId,
					ctx.Saga.BookingId,
					Reason = ctx.Saga.FailureReason ?? "Payment failed or booking confirmation timeout"
				}))
				.TransitionTo(CompensatingBooking)
			);

			During(WaitingForPaymentCompletion,
				When(PaymentCompleted)
				.Then(context =>
				{
					context.Saga.PaymentCompletedAt = DateTime.UtcNow;
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
			.Unschedule(PaymentTimeout)
				.ThenAsync(context => context.Publish(new ConfirmBookingCommand
				{
					CorrelationId = context.Saga.CorrelationId,
					BookingId = context.Saga.BookingId,
					PaymentId = context.Saga.PaymentId,
				}))
				.Schedule(BookingConfirmationTimeout, context => context.Init<BookingConfirmationTimeoutExpired>(new
				{
					context.Saga.CorrelationId,
					context.Saga.BookingId
				}))
				.TransitionTo(WaitingForBookingConfirmation),

				When(PaymentFailed)
				.Then(context =>
				{
					context.Saga.FailureReason = $"Payment failed: {context.Message.Reason}";
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.Unschedule(PaymentTimeout)
				.PublishAsync(ctx => ctx.Init<CancelBookingCommand>(new
				{
					ctx.Saga.CorrelationId,
					ctx.Saga.BookingId,
					Reason = ctx.Saga.FailureReason ?? "Payment failed or booking confirmation timeout"
				}))
				.TransitionTo(CompensatingBooking),

				When(PaymentTimeout!.Received)
					.Then(context =>
					{
						context.Saga.FailureReason = "Payment timeout - 15 minutes expired";
						context.Saga.UpdatedAt = DateTime.UtcNow;
					})
					.PublishAsync(ctx => ctx.Init<CancelBookingCommand>(new
					{
						ctx.Saga.CorrelationId,
						ctx.Saga.BookingId,
						Reason = ctx.Saga.FailureReason ?? "Payment failed or booking confirmation timeout"
					}))
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
				.ThenAsync(ctx => ctx.Publish(new SendBookingConfirmationNotificationCommand
				{
					CorrelationId = ctx.Saga.CorrelationId,
					RecipientId = ctx.Saga.CustomerId,
					BookingId = ctx.Saga.BookingId,
					CustomerEmail = ctx.Saga.CustomerEmail ?? string.Empty,
					ServiceDescription = ctx.Saga.ServiceDescription ?? string.Empty,
					Amount = ctx.Saga.Amount,
					Currency = ctx.Saga.Currency ?? "USD",
					PaymentReference = ctx.Saga.PaymentReference
				}))
				.TransitionTo(WaitingForNotification),
				
				When(BookingConfirmationTimeout!.Received)
				.Then(context =>
				{
					context.Saga.BookingConfirmationRetryCount++;
					context.Saga.FailureReason = "Booking confirmation timeout";
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.PublishAsync(ctx => ctx.Init<InitiateRefundCommand>(new
				{
					ctx.Saga.CorrelationId,
					PaymentId = ctx.Saga.PaymentId,
					Amount = ctx.Saga.Amount,
					Currency = ctx.Saga.Currency ?? "USD",
					RecipientEmail = ctx.Saga.CustomerEmail ?? string.Empty,
					Reason = ctx.Saga.FailureReason ?? "Booking confirmation timeout"
				}))
				.TransitionTo(CompensatingPayment)
			);

			During(WaitingForNotification,
				When(NotificationSent)
				.Then(context =>
				{
					context.Saga.CompletedAt = DateTime.UtcNow;
					Console.WriteLine($"SAGA saga-789: COMPLETE! ✅");
				})
				.Finalize()
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
				When(PaymentRefunded)
				.Then(context =>
				{
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.PublishAsync(context => context.Init<CancelBookingCommand>(new
				{
					context.Saga.CorrelationId,
					context.Saga.BookingId,
					Reason = context.Saga.FailureReason ?? "Payment failed or booking confirmation timeout"
				}))
				.TransitionTo(CompensatingBooking),

				When(PaymentFailed)
				.Then(context =>
				{
					context.Saga.UpdatedAt = DateTime.UtcNow;
				})
				.PublishAsync(context => context.Init<CancelBookingCommand>(new
				{
					context.Saga.CorrelationId,
					context.Saga.BookingId,
					Reason = context.Saga.FailureReason ?? "Refund processed, cancelling booking"
				}))
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
		public Event<RefundProcessedIntegrationEvent> PaymentRefunded { get; private set; }
		public Event<BookingConfirmedIntegrationEvent> BookingConfirmed { get; private set; }
		public Event<BookingCancelledIntegrationEvent> BookingCancelled { get; private set; }
		public Event<NotificationSentIntegrationEvent> NotificationSent { get; private set; }

		public Schedule<BookingToPaymentState, PaymentTimeoutExpired> PaymentTimeout { get; private set; }
		public Schedule<BookingToPaymentState, BookingConfirmationTimeoutExpired> BookingConfirmationTimeout { get; private set; }
	}
}
