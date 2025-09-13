using MediatR;

namespace PaymentManagement.Application.CQRS.Queries.InvoiceQueries
{
	public class GetDaysUntilDueQuery : IRequest<int>
	{
		public Guid InvoiceId { get; set; }
	}
}
