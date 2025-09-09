using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.DTOs.BookingDTOs;
using BookingManagement.Application.Validators.BookingValidators;
using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class UpdateBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<UpdateBookingCommandHandler> logger,
		IMapper mapper,
		IUnitOfWork unitOfWork) : IRequestHandler<UpdateBookingCommand, BookingResponseDTO>
	{
		public async Task<BookingResponseDTO> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
		{
			var response = new BookingResponseDTO();
			var validator = new BookingUpdateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.BookingDTO, cancellationToken);
			if(!validationResult.IsValid)
			{
				response.IsSuccess = false;
				response.Message = "Validation failed.";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				var booking = await bookingRepository.GetByIdAsync(request.BookingDTO.BookingId, cancellationToken)
				?? throw new KeyNotFoundException($"Booking with ID {request.BookingDTO.BookingId} not found.");
				mapper.Map(request.BookingDTO, booking);
				await bookingRepository.UpdateAsync(booking, cancellationToken);

				logger.LogInformation($"Booking with ID {booking.Id} updated successfully.");
				response = mapper.Map<BookingResponseDTO>(booking);
				response.IsSuccess = true;
				response.Message = "Booking updated successfully.";
			}, cancellationToken);
			
			return response;
		}
	}
}
