using MediatR;
using PaymentManagement.Application.DTOs.InvoiceDTOS;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class UpdateInvoiceCommand : IRequest<InvoiceResponseDTO>
	{
		public InvoiceUpdateDTO InvoiceDTO { get; set; }
	}
}
