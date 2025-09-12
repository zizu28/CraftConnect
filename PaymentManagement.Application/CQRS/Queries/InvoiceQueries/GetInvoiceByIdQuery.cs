using MediatR;
using PaymentManagement.Application.DTOs.InvoiceDTOS;

namespace PaymentManagement.Application.CQRS.Queries.InvoiceQueries
{
	public class GetInvoiceByIdQuery : IRequest<InvoiceResponseDTO>
	{
		public Guid InvoiceId { get; set; }
	}
}
