using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class CreatePaymentCommand : IRequest<PaymentResponseDTO>
	{
		public string RecipientEmail { get; set; }
		public PaymentCreateDTO PaymentDTO { get; set; }
		public decimal TransactionCharge { get; set; }
	}
}
