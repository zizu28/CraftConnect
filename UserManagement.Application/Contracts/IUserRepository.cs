using UserManagement.Domain.Entities;

namespace UserManagement.Application.Contracts
{
	public interface IUserRepository : IRepository<User>
	{
		Task<Dictionary<Guid, string>> GetNamesByIdsAsync(List<Guid> uniqueIds, CancellationToken cancellationToken);
	}
}
