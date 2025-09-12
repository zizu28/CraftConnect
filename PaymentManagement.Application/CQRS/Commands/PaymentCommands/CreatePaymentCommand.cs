using MediatR;
using PaymentManagement.Application.DTOs.PaymentDTOs;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class CreatePaymentCommand : IRequest<PaymentResponseDTO>
	{
		public string RecipientEmail { get; set; }
		public PaymentCreateDTO PaymentDTO { get; set; }
	}
}
