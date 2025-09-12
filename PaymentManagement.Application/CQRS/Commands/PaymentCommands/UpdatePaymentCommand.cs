using MediatR;
using PaymentManagement.Application.DTOs.PaymentDTOs;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class UpdatePaymentCommand : IRequest<PaymentResponseDTO>
	{
		public PaymentUpdateDTO PaymentDTO { get; set; }
	}
}
