using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserManagement.Application.Contracts;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class CraftmanRepository(ApplicationDbContext dbContext) : ICraftsmanRepository
	{
		public async Task AddAsync(Craftman entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			await dbContext.Craftmen.AddAsync(entity, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var craftman = await dbContext.Craftmen.FindAsync([id], cancellationToken)
				?? throw new KeyNotFoundException($"Craftman with ID {id} not found.");
			dbContext.Craftmen.Remove(craftman);
		}

		public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await dbContext.Craftmen.AnyAsync(c => c.Id == id, cancellationToken);
		}

		public async Task<Craftman> FindBy(Expression<Func<Craftman, bool>> predicate, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			var craftman = await dbContext.Craftmen.FirstOrDefaultAsync(predicate, cancellationToken);
			return craftman!;
		}

		public async Task<IEnumerable<Craftman>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await dbContext.Craftmen.AsNoTracking().ToListAsync(cancellationToken);
		}

		public Task<Craftman> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(email);
			return FindBy(c => c.Email.Address == email, cancellationToken);
		}

		public async Task<Craftman> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var craftman = await FindBy(c => c.Id == id, cancellationToken);
			return craftman;
		}

		public Task<List<Craftman>> GetByIdsAsync(List<Guid> uniqueIds, CancellationToken cancellationToken = default)
		{
			var craftmen = dbContext.Craftmen
				.Where(c => uniqueIds.Contains(c.Id))
				.ToListAsync(cancellationToken);
			return craftmen;
		}

		public async Task<Craftman> GetByProfessionAsync(string profession, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(profession);
			var craftman = await dbContext.Craftmen
				.FirstOrDefaultAsync(c => c.Profession.ToString().Equals(profession, StringComparison.OrdinalIgnoreCase), cancellationToken);
			return craftman ?? throw new KeyNotFoundException($"Craftman with profession '{profession}' not found.");
		}

		public async Task<Craftman> GetBySkillsAsync(CancellationToken cancellationToken = default, params string[] skills)
		{
			ArgumentNullException.ThrowIfNull(skills);
			if (skills.Length == 0)
			{
				throw new ArgumentException("At least one skill must be provided.", nameof(skills));
			}
			var craftman = await dbContext.Craftmen
				.Where(c => c.Skills.Any(s => skills.Contains(s.Name, StringComparer.OrdinalIgnoreCase)))
				.FirstOrDefaultAsync(cancellationToken);
			return craftman ?? throw new KeyNotFoundException("Craftman with specified skills not found.");
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task UpdateAsync(Craftman entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			dbContext.Craftmen.Update(entity);
		}
	}
}
