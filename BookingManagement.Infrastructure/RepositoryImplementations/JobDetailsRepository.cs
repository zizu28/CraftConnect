using BookingManagement.Application.Contracts;
using BookingManagement.Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookingManagement.Infrastructure.RepositoryImplementations
{
	public class JobDetailsRepository(ApplicationDbContext dbContext) : IJobDetailsRepository
	{
		public async Task AddAsync(JobDetails entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), "JobDetails entity cannot be null.");
			}
			await dbContext.JobDetails.AddAsync(entity, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var entity = await dbContext.JobDetails.FindAsync([id], cancellationToken)
				?? throw new KeyNotFoundException($"JobDetails with ID {id} not found.");
			dbContext.JobDetails.Remove(entity);
		}

		public async Task<JobDetails> FindBy(Expression<Func<JobDetails, bool>> predicate, CancellationToken cancellationToken = default)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null.");
			}
			return await dbContext.JobDetails.FirstOrDefaultAsync(predicate, cancellationToken)
				?? throw new KeyNotFoundException("No JobDetails found matching the specified criteria.");
		}

		public async Task<IEnumerable<JobDetails>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await dbContext.JobDetails.AsNoTracking().ToListAsync(cancellationToken)
				?? throw new KeyNotFoundException("No JobDetails found.");
		}

		public async Task<JobDetails> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var entity = await dbContext.JobDetails.FindAsync([id], cancellationToken)
				?? throw new KeyNotFoundException($"JobDetails with ID {id} not found.");
			return entity;
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public Task UpdateAsync(JobDetails entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), "JobDetails entity cannot be null.");
			}
			dbContext.JobDetails.Update(entity);
			return Task.CompletedTask;
		}
	}
}
