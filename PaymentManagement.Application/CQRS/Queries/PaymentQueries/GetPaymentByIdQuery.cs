using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Queries.PaymentQueries
{
	public class GetPaymentByIdQuery : IRequest<PaymentResponseDTO>
	{
		public Guid PaymentId { get; set; }
	}
}
