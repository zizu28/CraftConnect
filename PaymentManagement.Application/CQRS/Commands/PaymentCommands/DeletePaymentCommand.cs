using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class DeletePaymentCommand : IRequest<Unit>
	{
		public string Email { get; set; }
		public string DeletedBy { get; set; }
		public string Reason { get; set; }
		public Guid Id { get; set; }
	}
}
