using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.Validators;
using BookingManagement.Domain.Entities;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class BookingLineItemCreateCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<BookingLineItemCreateCommandHandler> logger,
		IMapper mapper,
		IMessageBroker messageBroker,
		IUnitOfWork unitOfWork)
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
			
			var lineItem = mapper.Map<BookingLineItem>(request.LineItemCreateDTO);
			var domainEvents = booking.DomainEvents.ToList();
			var bookingLineItemEvent = domainEvents.OfType<BookingLineItemIntegrationEvent>().FirstOrDefault();
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				booking.AddLineItem(lineItem.Description, lineItem.Price, lineItem.Quantity);

				await bookingRepository.UpdateAsync(booking, cancellationToken);
				if(bookingLineItemEvent != null)
					await messageBroker.PublishAsync(bookingLineItemEvent, cancellationToken);
				logger.LogInformation("Booking line item created successfully for Booking ID {BookingId}", request.BookingId);
				booking.ClearEvents();
			}, cancellationToken);
			response = mapper.Map<BookingLineItemResponseDTO>(lineItem);
			response.IsSuccess = true;
			response.Message = "Booking line item created successfully";
			return response;

		}
	}
}
