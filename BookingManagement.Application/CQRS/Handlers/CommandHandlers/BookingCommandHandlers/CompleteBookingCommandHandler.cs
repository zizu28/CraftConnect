using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.Validators.BookingValidators;
using Core.Logging;
using Core.SharedKernel.Domain;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class CompleteBookingCommandHandler(
		IDomainEventsDispatcher domainEventsDispatcher,
		ILoggingService<CompleteBookingCommandHandler> logger,
		IBookingRepository bookingRepository) : IRequestHandler<CompleteBookingCommand, Unit>
	{
		public async Task<Unit> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
		{
			var validator = new CompleteBookingCommandValidator();
			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Validation failed for CompleteBookingCommand: {Errors}", validationResult.Errors);
				return Unit.Value;
			}
			var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
				?? throw new InvalidOperationException($"Booking with ID {request.BookingId} not found.");
			booking.CompleteBooking();
			await domainEventsDispatcher.DispatchAsync(booking.DomainEvents, cancellationToken);
			await bookingRepository.UpdateAsync(booking, cancellationToken);
			await bookingRepository.SaveChangesAsync(cancellationToken);

			logger.LogInformation("Booking with ID {BookingId} has been completed successfully.", request.BookingId);
			booking.ClearEvents();
			return Unit.Value;
		}
	}
}
