using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.DTOs.BookingDTOs;
using BookingManagement.Application.Validators.BookingValidators;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class UpdateBookingDetailsCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<UpdateBookingDetailsCommandHandler> logger,
		IMessageBroker messageBroker) : IRequestHandler<UpdateBookingDetailsCommand, string>
	{
		public async Task<string> Handle(UpdateBookingDetailsCommand request, CancellationToken cancellationToken)
		{
			var validator = new BookingDetailsDTOValidator();
			var validationResult = await validator.ValidateAsync(request.BookingDetailsDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Failed validation of booking update entity.");
				return $"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}";
			}
			logger.LogInformation("Booking update metadata successfully validated.");
			var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
				?? throw new ApplicationException($"Booking with ID {request.BookingId} not found.");
			
			booking.UpdateJobDetails(request.BookingDetailsDTO.Description);

			await bookingRepository.UpdateAsync(booking, cancellationToken);

			var bookingUpdatedEvent = new BookingUpdatedIntegrationEvent(booking.Id,
				booking.CustomerId, booking.CraftmanId, booking.StartDate, booking.EndDate,
				booking.Status, booking.CalculateTotalPrice(), new LocalDateTime(DateTime.UtcNow.Year,
				DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute));

			await messageBroker.PublishAsync(bookingUpdatedEvent, cancellationToken);
			await bookingRepository.SaveChangesAsync(cancellationToken);
			
			booking.ClearEvents();
			logger.LogInformation($"Booking with ID {request.BookingId} updated successfully.");
			return booking.Id.ToString();
		}
	}
}
