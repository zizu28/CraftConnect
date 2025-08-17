using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.DTOs.BookingLineItemDTOs;
using BookingManagement.Application.Validators;
using Core.Logging;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingLineItemCommandHandlers
{
	public class BookingLineItemCreateCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<BookingLineItemCreateCommandHandler> logger,
		IMapper mapper)
		: IRequestHandler<BookingLineItemCreateCommand, BookingLineItemResponseDTO>
	{
		public async Task<BookingLineItemResponseDTO> Handle(BookingLineItemCreateCommand request, CancellationToken cancellationToken)
		{
			var response = new BookingLineItemResponseDTO();
			var validator = new BookingLineItemCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.LineItemCreateDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Validation failed for BookingLineItemCreateCommand: {Errors}", validationResult.Errors);
				response.IsSuccess = false;
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				response.Message = "Validation failed";
				return response;
			}
			var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
				?? throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");
			booking.AddLineItem(request.LineItemCreateDTO.Description,
				request.LineItemCreateDTO.Price, request.LineItemCreateDTO.Quantity);
			await bookingRepository.UpdateAsync(booking, cancellationToken);
			await bookingRepository.SaveChangesAsync(cancellationToken);
			//await domainEventsDispatcher.DispatchAsync(booking.DomainEvents, cancellationToken);
			logger.LogInformation("Booking line item created successfully for Booking ID {BookingId}", request.BookingId);
			booking.ClearEvents();
			response = mapper.Map<BookingLineItemResponseDTO>(booking);
			response.IsSuccess = true;
			response.Message = "Booking line item created successfully";
			return response;

		}
	}
}
