using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Domain.Entities;
using System.Linq.Expressions;

namespace ProductInventoryManagement.Infrastructure.RepositoryImplementations
{
	public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
	{
		public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(entity.Name, nameof(entity.Name));
			await dbContext.Products.AddAsync(entity, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var existingPoduct = await GetByIdAsync(id, cancellationToken);
			if(existingPoduct == null)
			{

			}
			dbContext.Products.Remove(existingPoduct!);
		}

		public async Task<Product> FindBy(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
		{
			var product = await dbContext.Products.FirstOrDefaultAsync(predicate, cancellationToken);
			return product!;
		}

		public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var products = await dbContext.Products
				.AsNoTracking()
				.ToListAsync(cancellationToken);
			return products;
		}

		public async Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var product = await dbContext.Products
				.Include(p => p.Inventory)
				.Include(p => p.Images)
				.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
			return product!;
		}

		public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
		{
			var products = await dbContext.Products
				.Where(p => p.CategoryId == categoryId)
				.ToListAsync(cancellationToken);
			return products;
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
		{
			var products = await dbContext.Products
				.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
				.AsNoTracking()
				.ToListAsync(cancellationToken);
			return products;
		}

		public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(entity.Name, nameof(entity.Name));
			dbContext.Products.Update(entity);
			await Task.CompletedTask;
		}
	}
}
