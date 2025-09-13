using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class DeleteBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<DeleteBookingCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork) : IRequestHandler<DeleteBookingCommand, Unit>
	{
		public async Task<Unit> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
		{
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
				?? throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");
				booking.CancelBooking(request.Reason);

				await bookingRepository.DeleteAsync(booking.Id, cancellationToken);

				var bookingCancelledEvent = new BookingCancelledIntegrationEvent(booking.Id, request.Reason);
				await messageBroker.PublishAsync(bookingCancelledEvent, cancellationToken);

				logger.LogInformation($"Booking with ID {request.BookingId} deleted successfully.");
				booking.ClearEvents();

				backgroundJob.Enqueue<IMessageBroker>(
					"delete-booking-event",
					broker => broker.SendAsync("delete-booking", new BookingCancelledIntegrationEvent(
												booking.Id, request.Reason), cancellationToken));
			}, cancellationToken);
			

			return Unit.Value;
		}
	}
}
