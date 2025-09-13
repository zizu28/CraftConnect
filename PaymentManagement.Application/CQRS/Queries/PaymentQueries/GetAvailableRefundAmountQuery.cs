using Core.SharedKernel.ValueObjects;
using MediatR;

namespace PaymentManagement.Application.CQRS.Queries.PaymentQueries
{
	public class GetAvailableRefundAmountQuery : IRequest<Money>
	{
		public Guid PaymentId { get; set; }
	}
}
