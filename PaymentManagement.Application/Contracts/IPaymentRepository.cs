using Core.SharedKernel.Contracts;
using PaymentManagement.Domain.Entities;
using PayStack.Net;

namespace PaymentManagement.Application.Contracts
{
	public interface IPaymentRepository : IRepository<Payment>
	{
	}
}
