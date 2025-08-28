using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.DTOs.JobDetailsDTOs;
using BookingManagement.Application.Validators.JobDetailsValidators;
using BookingManagement.Domain.Entities;
using Core.Logging;
using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class CreateJobDetailsCommandHandler(
		IBookingRepository bookingRepository,
		ApplicationDbContext dbContext,
		ILoggingService<CreateJobDetailsCommandHandler> logger) : IRequestHandler<CreateBookingDetailsCommand, JobDetailsResponseDTO>
	{
		public async Task<JobDetailsResponseDTO> Handle(CreateBookingDetailsCommand request, CancellationToken cancellationToken)
		{
			var response = new JobDetailsResponseDTO();
			var validator = new JobDetailsCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.JobDetails, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Failed validation of booking create entity.");
				response.IsSuccess = false;
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				response.Message = "Validation failed.";
				return response;
			}
			logger.LogInformation("Booking create metadata successfully validated.");

			var booking = await bookingRepository.GetByIdAsync(request.JobDetails.BookingId, cancellationToken)
				?? throw new KeyNotFoundException($"Booking with ID {request.JobDetails.BookingId} not found.");
			logger.LogInformation("Booking found in the database.");
			booking.AddJobDetails(request.JobDetails.Description);
			await bookingRepository.UpdateAsync(booking, cancellationToken);
			try
			{
				await bookingRepository.SaveChangesAsync(cancellationToken);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				foreach(var entry in ex.Entries)
				{
					if(entry.Entity is JobDetails)
					{
						logger.LogWarning("Conflicting Entry - Entity Type: {EntityType}, State: {State}",
							entry.Metadata.ClrType?.Name ?? "Unknown", entry.State);

						// Log original vs current values for the concurrency token if possible
						// This might be tricky with xmin, but worth trying for other properties
						// var originalValues = entry.OriginalValues;
						// var currentValues = entry.CurrentValues;
						// Log property differences...
					}

					// Re-throw or handle appropriately
					// For owned entities, it's often best to treat it as a general conflict
					throw new ApplicationException("The booking was modified by another process. Please try again.");
					// Or return a failed response DTO
					// response.IsSuccess = false;
					// response.Message = "The booking was modified by another process. Please reload and try again.";
					// return response;
				}
			}
			
			response.BookingId = booking.Id;
			response.Description = booking.Details.Description ?? string.Empty;
			response.IsSuccess = true;
			response.Message = "Booking created successfully.";
			return response;
		}
	}
}
