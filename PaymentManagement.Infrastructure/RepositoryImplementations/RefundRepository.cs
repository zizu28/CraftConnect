using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Infrastructure.RepositoryImplementations
{
	public class RefundRepository(
		ApplicationDbContext dbContext) : BaseRepository<Refund>(dbContext), IRefundRepository
	{
		public async Task<Refund?> GetByExternalIdAsync(string refundId)
		{
			var refund = await dbContext.Refunds
				.AsNoTracking()
				.FirstOrDefaultAsync(r => r.ExternalRefundId == refundId)
				?? throw new KeyNotFoundException($"Refund with ExternalRefundId '{refundId}' not found.");
			return refund;
		}
	}
}
