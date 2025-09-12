using MediatR;
using PaymentManagement.Application.DTOs.InvoiceDTOS;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class CreateInvoiceCommand : IRequest<InvoiceResponseDTO>
	{
		public InvoiceCreateDTO InvoiceDTO { get; set; }
	}
}
