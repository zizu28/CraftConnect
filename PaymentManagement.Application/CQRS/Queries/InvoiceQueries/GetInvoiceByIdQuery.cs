using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Queries.InvoiceQueries
{
	public class GetInvoiceByIdQuery : IRequest<InvoiceResponseDTO>
	{
		public Guid InvoiceId { get; set; }
	}
}
