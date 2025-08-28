using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.Validators.JobDetailsValidators;
using Core.Logging;
using MediatR;
using System.Data;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class UpdateBookingDetailsCommandHandler(
		//IJobDetailsRepository jobDetailsRepository,
		IBookingRepository bookingRepository,
		ILoggingService<UpdateBookingDetailsCommandHandler> logger) : IRequestHandler<UpdateBookingDetailsCommand, string>
	{
		public async Task<string> Handle(UpdateBookingDetailsCommand request, CancellationToken cancellationToken)
		{
			var validator = new JobDetailsUpdateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.JobDetails, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Failed validation of booking update entity.");
				return $"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}";
			}
			logger.LogInformation("Booking update metadata successfully validated.");

			var booking = await bookingRepository.GetByIdAsync(request.JobDetails.BookingId, cancellationToken);
			if (booking == null)
			{
				logger.LogWarning($"Booking with ID {request.JobDetails.BookingId} not found.");
				return $"Booking with ID {request.JobDetails.BookingId} not found.";
			}
			logger.LogInformation("Booking found in the database.");
			booking.UpdateJobDetails(request.JobDetails.Description);
			await bookingRepository.UpdateAsync(booking, cancellationToken);
			

			try
			{
				await bookingRepository.SaveChangesAsync(cancellationToken);
			}
			catch (DBConcurrencyException ex)
			{
				
			}
			logger.LogInformation("Booking details updated successfully in the database.");
			return "Booking details updated successfully.";
		}
	}
}
