using BookingManagement.Application.DTOs.JobDetailsDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class CreateBookingDetailsCommand : IRequest<JobDetailsResponseDTO>
	{
		public JobDetailsCreateDTO JobDetails { get; set; }
	}
}
