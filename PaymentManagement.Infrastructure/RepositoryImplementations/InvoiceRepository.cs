using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Infrastructure.RepositoryImplementations
{
	public class InvoiceRepository(ApplicationDbContext dbContext) : BaseRepository<Invoice>(dbContext), IInvoiceRepository
	{
		public override async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _dbSet
				.AsNoTracking()
				.Include(i => i.LineItems)
				.Include(i => i.Payments)
				.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
		}
	}
}
