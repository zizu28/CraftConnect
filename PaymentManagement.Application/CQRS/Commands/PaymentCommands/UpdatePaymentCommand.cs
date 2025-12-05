using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class UpdatePaymentCommand : IRequest<PaymentResponseDTO>
	{
		public PaymentUpdateDTO PaymentDTO { get; set; }
	}
}
