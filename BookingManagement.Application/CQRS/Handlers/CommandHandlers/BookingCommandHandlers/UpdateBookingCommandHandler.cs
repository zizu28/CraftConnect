using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.DTOs.BookingDTOs;
using BookingManagement.Application.Validators.BookingValidators;
using Core.Logging;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class UpdateBookingCommandHandler(
		IBookingRepository bookingRepository,
		IMapper mapper,
		ILoggingService<UpdateBookingCommandHandler> logger) : IRequestHandler<UpdateBookingCommand, BookingResponseDTO>
	{
		public async Task<BookingResponseDTO> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
		{
			var response = new BookingResponseDTO();
			var validator = new BookingUpdateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.BookingDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Failed validation of booking update entity.");
				response.IsSuccess = false;
				response.Message = $"Booking update validation failed";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}
			logger.LogInformation("Booking update metadata successfully validated.");
			var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
				?? throw new ApplicationException($"Booking with ID {request.BookingId} not found.");
			mapper.Map(request.BookingDTO, booking);

			await bookingRepository.UpdateAsync(booking, cancellationToken);
			await bookingRepository.SaveChangesAsync(cancellationToken);

			response = mapper.Map<BookingResponseDTO>(booking);
			response.Message = $"Booking with ID {response.BookingId} succesfully updated.";
			response.IsSuccess = true;

			return response;
		}
	}
}
