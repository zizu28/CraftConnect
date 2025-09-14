using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.Validators.BookingValidators;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class CompleteBookingCommandHandler(
		ILoggingService<CompleteBookingCommandHandler> logger,
		IBookingRepository bookingRepository,
		IUnitOfWork unitOfWork) : IRequestHandler<CompleteBookingCommand, Unit>
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
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				booking.CompleteBooking();
				await bookingRepository.UpdateAsync(booking, cancellationToken);
				logger.LogInformation("Booking with ID {BookingId} has been completed successfully.", request.BookingId);
			}, cancellationToken);
			
			return Unit.Value;
		}
	}
}
