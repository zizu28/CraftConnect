using Core.SharedKernel.DTOs;
using MediatR;

namespace PaymentManagement.Application.CQRS.Queries.PaymentQueries
{
	public class GetAllPaymentsQuery : IRequest<IEnumerable<PaymentResponseDTO>>
	{
	}
}
