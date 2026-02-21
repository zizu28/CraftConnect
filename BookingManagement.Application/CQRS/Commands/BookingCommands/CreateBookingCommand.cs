using Core.SharedKernel.DTOs;
using Core.SharedKernel.ValueObjects;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class CreateBookingCommand : IRequest<BookingResponseDTO>
	{
		public BookingCreateDTO BookingDTO { get; set; }
		public GeoLocation Location { get; set; }
		public Guid CorrelationId { get; set; }
	}
}
