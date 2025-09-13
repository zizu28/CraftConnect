using Core.SharedKernel.ValueObjects;
using MediatR;

namespace PaymentManagement.Application.CQRS.Queries.InvoiceQueries
{
	public class GetOutstandingAmountQuery : IRequest<Money>
	{
		public Guid InvoiceId { get; set; }
	}
}
