using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class UpdateDueDateCommand : IRequest
	{
		public Guid InvoiceId { get; set; }
		public DateTime UpdatedDueDate { get; set; }
	}
}
