using MediatR;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.CQRS.Queries.PaymentQueries
{
	public class GetAllPaymentTransactionsQuery : IRequest<IEnumerable<PaymentTransaction>>
	{
		public Guid PaymentId { get; set; }
	}
}
