using Core.SharedKernel.Contracts;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.Contracts
{
	public interface IPaymentRepository : IRepository<Payment>
	{
	}
}
