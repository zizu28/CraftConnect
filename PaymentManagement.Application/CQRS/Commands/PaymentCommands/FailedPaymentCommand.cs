using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class FailedPaymentCommand : IRequest<bool>
	{
		public Guid PaymentId { get; set; }
		public string RecipientEmail { get; set; }
		public string FailureReason { get; set; }
	}
}
