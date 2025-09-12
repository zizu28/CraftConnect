using MediatR;
using PaymentManagement.Application.DTOs.InvoiceDTOS;

namespace PaymentManagement.Application.CQRS.Queries.InvoiceQueries
{
	public class GetAllInvoicesQuery : IRequest<IEnumerable<InvoiceResponseDTO>>
	{
	}
}
