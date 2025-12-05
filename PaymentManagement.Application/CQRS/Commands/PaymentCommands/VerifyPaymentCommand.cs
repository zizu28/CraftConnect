using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class VerifyPaymentCommand : IRequest<(string Message, bool Status)>
	{
		public string Reference { get; set; }
		public PaymentUpdateDTO PaymentDTO { get; set; }
	}
}
