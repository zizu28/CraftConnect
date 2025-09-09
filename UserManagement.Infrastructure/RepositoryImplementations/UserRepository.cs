using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserManagement.Application.Contracts;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
	{
		public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			await dbContext.Users.AddAsync(entity, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var user = await dbContext.Users.FindAsync( [id], cancellationToken) ?? throw new KeyNotFoundException($"User with ID {id} not found.");
			dbContext.Users.Remove(user);
		}

		public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await dbContext.Users.AnyAsync(u => u.Id == id, cancellationToken);
		}

		public async Task<User?> FindBy(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
		}

		public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
		}

		public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
		{
			return await FindBy(u => u.Email.Address == email, cancellationToken);
		}

		public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await FindBy(u => u.Id == id, cancellationToken);
		}

		public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			dbContext.Users.Update(entity);
			return Task.CompletedTask;
		}
	}
}
