using UserManagement.Domain.Entities;

namespace UserManagement.Application.Contracts
{
	public interface ICraftsmanRepository : IRepository<Craftman>
	{
		Task<Craftman> GetByProfessionAsync(string profession, CancellationToken cancellationToken = default);
		Task<Craftman> GetBySkillsAsync(CancellationToken cancellationToken = default, params string[] skills);
		Task<List<Craftman>> GetByIdsAsync(List<Guid> uniqueIds, CancellationToken cancellationToken = default);
	}
}
