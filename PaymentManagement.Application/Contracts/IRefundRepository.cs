using Core.SharedKernel.Contracts;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.Contracts
{
	public interface IRefundRepository : IRepository<Refund>
	{
		Task<Refund?> GetByExternalIdAsync(string refundId);
	}
}
