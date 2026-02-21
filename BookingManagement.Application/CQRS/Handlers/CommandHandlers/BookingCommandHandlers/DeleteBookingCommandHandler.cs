using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
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
				booking.CancelBooking(request.CorrelationId, request.Reason);

				await bookingRepository.DeleteAsync(booking.Id, cancellationToken);
				var domainEvents = booking.DomainEvents.ToList();
				var cancelledEvent = domainEvents.OfType<BookingCancelledIntegrationEvent>().FirstOrDefault();

				if (cancelledEvent != null) 
				{
					await messageBroker.PublishAsync(cancelledEvent, cancellationToken);
				}
				
				logger.LogInformation($"Booking with ID {request.BookingId} deleted successfully.");
				booking.ClearEvents();

				backgroundJob.Enqueue<IGmailService>(
					"delete-booking-event",
					broker => broker.SendEmailAsync(
						request.RecipientEmail,
						"BOOKING CANCELLED",
						$"Your booking with ID {request.BookingId} has been cancelled.",
						false,
						CancellationToken.None
						));
			}, cancellationToken);
			

			return Unit.Value;
		}
	}
}
