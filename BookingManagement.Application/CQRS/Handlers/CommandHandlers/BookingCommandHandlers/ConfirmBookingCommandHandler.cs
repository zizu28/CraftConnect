using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.Validators.BookingValidators;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class ConfirmBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<ConfirmBookingCommandHandler> logger,
		IUnitOfWork unitOfWork,
		IMessageBroker messageBroker) : IRequestHandler<ConfirmBookingCommand, Unit>
	{
		public async Task<Unit> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
		{
			var validator = new ConfirmBookingCommandValidator();
			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogError(new Exception(), "Invalid booking command for confirmation");
				throw new Exception("Invalid booking command for confirmation");
			}

			var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
					?? throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");

			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				// Guard must be checked BEFORE calling ConfirmBooking() — the domain method
				// changes Status to Confirmed, so checking it after always throws.
				if (booking.Status != BookingStatus.Pending)
				{
					throw new InvalidOperationException($"Booking with ID {request.BookingId} is not in a pending state.");
				}

				// Domain call: raises BookingConfirmedIntegrationEvent using the SAGA's correlationId
				booking.ConfirmBooking(request.CorrelationId);

				var confirmBookingEvent = booking
					.DomainEvents
					.OfType<BookingConfirmedIntegrationEvent>()
					.FirstOrDefault(e => e.BookingId == request.BookingId);

				await bookingRepository.UpdateAsync(booking, cancellationToken);

				if (confirmBookingEvent != null)
					await messageBroker.PublishAsync(confirmBookingEvent, cancellationToken);

				logger.LogInformation($"Booking with ID {request.BookingId} confirmed at {request.ConfirmedAt}.");
			}, cancellationToken);
			
			return Unit.Value;
		}
	}
}
