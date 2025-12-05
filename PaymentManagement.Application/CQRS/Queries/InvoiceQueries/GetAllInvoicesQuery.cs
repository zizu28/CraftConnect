using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Queries.InvoiceQueries
{
	public class GetAllInvoicesQuery : IRequest<IEnumerable<InvoiceResponseDTO>>
	{
	}
}
