using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class CancelPaymentCommand : IRequest<Unit>
	{
		public Guid CorrelationId { get; set; }
		public Guid PaymentId { get; set; }
		public string Reason { get; set; } = string.Empty;
	}
}
