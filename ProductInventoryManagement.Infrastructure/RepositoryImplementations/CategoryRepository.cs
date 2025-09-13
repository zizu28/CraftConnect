using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Infrastructure.RepositoryImplementations
{
	public class CategoryRepository(ApplicationDbContext dbContext) : BaseRepository<Category>(dbContext), ICategoryRepository
	{
		public override async Task<Category> AddAsync(Category entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			ArgumentException.ThrowIfNullOrWhiteSpace(entity.Name, nameof(entity.Name));

			var existsWithSameName = await _dbSet
				.AnyAsync(c => c.Name.ToLower() == entity.Name.ToLower(), cancellationToken);
			
			if (existsWithSameName)
			{
				throw new InvalidOperationException($"Category with name '{entity.Name}' already exists");
			}

			var category = await base.AddAsync(entity, cancellationToken);
			return category;
		}

		public override async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _dbSet
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		public async Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
			
			return await _dbSet
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
		}

		public override async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			ArgumentException.ThrowIfNullOrWhiteSpace(entity.Name, nameof(entity.Name));

			var existsWithSameName = await _dbSet
				.AnyAsync(c => c.Name.ToLower() == entity.Name.ToLower() && c.Id != entity.Id, cancellationToken);
			
			if (existsWithSameName)
			{
				throw new InvalidOperationException($"Category with name '{entity.Name}' already exists");
			}

			await base.UpdateAsync(entity, cancellationToken);
		}

		public async Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default)
		{
			return await _dbSet
				.AsNoTracking()
				.Where(c => c.ParentCategoryId == parentCategoryId)
				.ToListAsync(cancellationToken);
		}

		public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
		{
			return await _dbSet
				.AsNoTracking()
				.Where(c => c.ParentCategoryId == null)
				.ToListAsync(cancellationToken);
		}

		Task IRepository<Category>.AddAsync(Category entity, CancellationToken cancellationToken)
		{
			return AddAsync(entity, cancellationToken);
		}
	}
}
