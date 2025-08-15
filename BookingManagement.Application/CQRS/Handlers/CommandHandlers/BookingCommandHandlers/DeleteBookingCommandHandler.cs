using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Events;
using Core.SharedKernel.IntegrationEvents;
using Infrastructure.BackgroundJobs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class DeleteBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<DeleteBookingCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob) : IRequestHandler<DeleteBookingCommand, Unit>
	{
		public async Task<Unit> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
		{
			var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
				?? throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");
			booking.CancelBooking(request.Reason);

			//await domainEventsDispatcher.DispatchAsync(booking.DomainEvents, cancellationToken);
			await bookingRepository.DeleteAsync(booking.Id, cancellationToken);

			var bookingCancelledEvent = new BookingCancelledIntegrationEvent(booking.Id, request.Reason);
			await messageBroker.PublishAsync(bookingCancelledEvent, cancellationToken);

			await bookingRepository.SaveChangesAsync(cancellationToken);
			logger.LogInformation($"Booking with ID {request.BookingId} deleted successfully.");
			booking.ClearEvents();

			backgroundJob.Enqueue<IMessageBroker>(
				"delete-booking-event",
				broker => broker.SendAsync("delete-booking", new BookingCancelledEvent(
											booking.Id, request.Reason), cancellationToken));

			return Unit.Value;
		}
	}
}
