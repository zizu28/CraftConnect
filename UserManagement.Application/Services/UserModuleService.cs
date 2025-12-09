using Core.SharedKernel.Contracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Contracts;

namespace UserManagement.Application.Services
{
	public class UserModuleService(
		ICraftsmanRepository repo,
		ApplicationDbContext dbContext) : IUserModuleService
	{
		private readonly ICraftsmanRepository _repo = repo;
		private readonly ApplicationDbContext _dbContext = dbContext;

		public async Task<string?> GetCraftsmanNameAsync(Guid craftsmanId, CancellationToken ct = default)
		{
			var name = await _dbContext.Users
			.Where(u => u.Id == craftsmanId)
			.Select(u => u.FirstName + " " + u.LastName)
			.FirstOrDefaultAsync(ct);

			return name;
		}

		public async Task<Dictionary<Guid, string>> GetCraftsmanNamesAsync(IEnumerable<Guid> craftsmanIds, CancellationToken ct = default)
		{
			var uniqueIds = craftsmanIds.Distinct().ToList();
			if (!(uniqueIds.Count > 0)) return [];
			var result = new Dictionary<Guid, string>();
			var craftsmen = await _repo.GetByIdsAsync(uniqueIds, ct); 
			foreach (var craftsman in craftsmen)
			{
				result[craftsman.Id] = $"{craftsman.FirstName} {craftsman.LastName}";
			}
			return result;
		}

		public async Task<string?> GetCustomerNameAsync(Guid customerId, CancellationToken ct = default)
		{
			var name = await _dbContext.Users
			.Where(u => u.Id == customerId)
			.Select(u => u.FirstName + " " + u.LastName)
			.FirstOrDefaultAsync(ct);

			return name;
		}

		public async Task<bool> IsCraftsmanValidAsync(Guid craftsmanId, CancellationToken ct = default)
		{
			return await _repo.ExistsAsync(craftsmanId, ct);
		}
	}
}
