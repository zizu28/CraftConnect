using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class CreatePaymentCommand : IRequest<PaymentResponseDTO>
	{
		public string RecipientEmail { get; set; }
		public string CallbackUrl { get; set; } = "https://localhost:7222/";
		public PaymentCreateDTO PaymentDTO { get; set; }
		public decimal TransactionCharge { get; set; }
	}
}
