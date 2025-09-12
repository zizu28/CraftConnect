using MediatR;
using PaymentManagement.Application.DTOs.PaymentDTOs;

namespace PaymentManagement.Application.CQRS.Queries.PaymentQueries
{
	public class GetPaymentByIdQuery : IRequest<PaymentResponseDTO>
	{
		public Guid PaymentId { get; set; }
	}
}
