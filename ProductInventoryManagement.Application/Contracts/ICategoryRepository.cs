using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Application.Contracts
{
	public interface ICategoryRepository : IRepository<Category>
	{
		Task<Category> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default);
	}
}
