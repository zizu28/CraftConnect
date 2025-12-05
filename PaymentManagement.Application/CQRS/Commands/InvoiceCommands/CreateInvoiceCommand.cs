using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class CreateInvoiceCommand : IRequest<InvoiceResponseDTO>
	{
		public InvoiceCreateDTO InvoiceDTO { get; set; }
	}
}
