using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.DTOs.BookingDTOs;
using BookingManagement.Application.Validators.BookingValidators;
using BookingManagement.Domain.Entities;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents;
using Core.SharedKernel.ValueObjects;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class CreateBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<CreateBookingCommandHandler> logger,
		IMapper mapper,
		IMessageBroker publisher) : IRequestHandler<CreateBookingCommand, BookingResponseDTO>
	{
		public async Task<BookingResponseDTO> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
		{
			var response = new BookingResponseDTO();
			var validator = new BookingCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.BookingDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Failed validation of booking entity.");
				response.IsSuccess = false;
				response.Message = $"Booking validation failed";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}
			logger.LogInformation("Booking creation metadata successfully validated.");
			var booking = Booking.Create(request.BookingDTO.CustomerId,
								request.BookingDTO.CraftmanId,
								new Address(request.BookingDTO.Street, request.BookingDTO.City, request.BookingDTO.PostalCode),
								request.BookingDTO.InitialDescription,
								request.BookingDTO.StartDate, request.BookingDTO.EndDate);

			var bookingCreatedIntegrationEvent = new BookingRequestedIntegrationEvent
			{
				BookingId = booking.Id,
				CustomerId = booking.CustomerId,
				CraftspersonId = booking.CraftmanId,
				JobDescription = booking.Details.Description,
				ServiceAddress = booking.ServiceAddress.ToString()
			};

			await bookingRepository.AddAsync(booking, cancellationToken);
			await publisher.PublishAsync(bookingCreatedIntegrationEvent, cancellationToken);
			
			await bookingRepository.SaveChangesAsync(cancellationToken);

			booking.ClearEvents();
			logger.LogInformation("Booking with ID {BookingId} successfully created.", booking.Id);

			response = mapper.Map<BookingResponseDTO>(booking);
			response.Details.Desciption = booking.Details.Description;
			response.Message = $"Booking with ID {response.BookingId} succesfully added.";
			response.IsSuccess = true;

			return response;
		}
	}
}
