using Core.Logging;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Domain.Entities;
using PayStack.Net;

namespace PaymentManagement.Infrastructure.RepositoryImplementations
{
	public class PaymentRepository(ApplicationDbContext dbContext,
		ILoggingService<PaymentRepository> logger,
		IConfiguration config) : BaseRepository<Payment>(dbContext), IPaymentRepository
	{
		public Task<TransactionInitializeResponse> Initialize(string email, int amount)
		{
			throw new NotImplementedException();
		}

		public Task<TransactionInitializeResponse> Initialize(TransactionInitializeRequest request)
		{
			throw new NotImplementedException();
		}

		public Task<TransactionVerifyResponse> Verify(string reference)
		{
			throw new NotImplementedException();
		}

		public override Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return _dbContext.Payments
				.Where(p => !p.IsDeleted)
				.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
		}
	}
}
