using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CustomerProjectCommands
{
	public class AddAttachmentCommand : IRequest<bool>
	{
		public Guid ProjectId { get; set; }
		public Guid DocumentId { get; set; } // ID from FileStorageService
	}
}
