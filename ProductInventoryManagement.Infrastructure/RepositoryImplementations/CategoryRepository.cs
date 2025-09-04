using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Domain.Entities;
using System.Linq.Expressions;

namespace ProductInventoryManagement.Infrastructure.RepositoryImplementations
{
	public class CategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
	{
		public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(entity.Name, nameof(entity.Name));
			await dbContext.Categories.AddAsync(entity, cancellationToken);
		}
		public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
		public async Task<Category> FindBy(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
		{
			var category = await dbContext.Categories.FirstOrDefaultAsync(predicate, cancellationToken);
			return category!;
		}
		public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var categories = await dbContext.Categories
				.AsNoTracking()
				.ToListAsync(cancellationToken);
			return categories;
		}
		public async Task<Category> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var category = await dbContext.Categories
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
			return category!;
		}
		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}
		public async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(entity.Name, nameof(entity.Name));
			dbContext.Categories.Update(entity);
			await Task.CompletedTask;
		}
	}
}
