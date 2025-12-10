using Core.SharedKernel.DTOs;

namespace Core.SharedKernel.Contracts
{
	public interface IUserModuleService
	{
		Task<string?> GetCraftsmanNameAsync(Guid craftsmanId, CancellationToken ct = default);
		Task<Dictionary<Guid, CraftsmanSummaryDto>> GetCraftsmanSummariesAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
		Task<string?> GetCustomerNameAsync(Guid customerId, CancellationToken ct = default);
		Task<Dictionary<Guid, string>> GetCraftsmanNamesAsync(IEnumerable<Guid> craftsmanIds, CancellationToken ct = default);
		Task<bool> IsCraftsmanValidAsync(Guid craftsmanId, CancellationToken ct = default);
	}
}
