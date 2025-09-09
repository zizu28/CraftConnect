using Core.SharedKernel.Contracts;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Application.Contracts
{
	public interface ICategoryRepository : IRepository<Category>
	{
		Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default);
		Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
		Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
	}
}
