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

		public async Task<User> FindBy(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			var user = await dbContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);
			return user!;
		}

		public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
		}

		public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
		{
			var user = await FindBy(u => u.Email.Address == email, cancellationToken);
			return user;
		}

		public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var user = await FindBy(u => u.Id == id, cancellationToken);
			return user;
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			dbContext.Users.Update(entity);
		}
	}
}
