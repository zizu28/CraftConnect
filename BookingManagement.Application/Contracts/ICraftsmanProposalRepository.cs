using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums;

namespace BookingManagement.Application.Contracts
{
	public interface ICraftsmanProposalRepository : IRepository<CraftsmanProposal>
	{
		Task<IEnumerable<CraftsmanProposal>> GetProposalsByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

		Task<IEnumerable<CraftsmanProposal>> GetProposalsByCraftsmanAsync(Guid craftsmanId, CancellationToken cancellationToken = default);

		Task<bool> HasCraftsmanAppliedAsync(Guid projectId, Guid craftsmanId, CancellationToken cancellationToken = default);

		Task<IEnumerable<CraftsmanProposal>> GetPendingProposalsByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
	}
}