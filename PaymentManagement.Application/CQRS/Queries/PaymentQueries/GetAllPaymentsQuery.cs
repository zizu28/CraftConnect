using MediatR;
using PaymentManagement.Application.DTOs.PaymentDTOs;

namespace PaymentManagement.Application.CQRS.Queries.PaymentQueries
{
	public class GetAllPaymentsQuery : IRequest<IEnumerable<PaymentResponseDTO>>
	{
	}
}
