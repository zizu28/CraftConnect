using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class DeletePaymentCommand : IRequest<Guid>
	{
		public Guid Id { get; set; }
	}
}
