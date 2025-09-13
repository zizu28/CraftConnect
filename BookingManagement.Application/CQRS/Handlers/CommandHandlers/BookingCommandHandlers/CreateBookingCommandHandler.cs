using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.DTOs.BookingDTOs;
using BookingManagement.Application.Validators.BookingValidators;
using BookingManagement.Domain.Entities;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class CreateBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<CreateBookingCommandHandler> logger,
		IMapper mapper,
		IMessageBroker publisher,
		IUnitOfWork unitOfWork) : IRequestHandler<CreateBookingCommand, BookingResponseDTO>
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
								request.BookingDTO.InitialDescription, request.BookingDTO.StartDate, request.BookingDTO.EndDate);

			var bookingCreatedIntegrationEvent = new BookingRequestedIntegrationEvent
			{
				BookingId = booking.Id,
				CraftspersonId = booking.CraftmanId,
				ServiceAddress = booking.ServiceAddress,
				Description = booking.Details.Description,
				Location = new GeoLocation(request.Location.Latitude, request.Location.Longitude)
			};

			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				await bookingRepository.AddAsync(booking, cancellationToken);
				await publisher.PublishAsync(bookingCreatedIntegrationEvent, cancellationToken);
				await publisher.SendAsync("booking-created-event", bookingCreatedIntegrationEvent, cancellationToken);
			}, cancellationToken);

			booking.ClearEvents();
			logger.LogInformation("Booking with ID {BookingId} successfully created.", booking.Id);

			response = mapper.Map<BookingResponseDTO>(booking);
			response.Message = $"Booking with ID {response.BookingId} succesfully added.";
			response.IsSuccess = true;

			return response;
		}
	}
}
