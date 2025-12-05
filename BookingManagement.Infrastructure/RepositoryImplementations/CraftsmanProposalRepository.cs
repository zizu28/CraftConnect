using BookingManagement.Application.Contracts;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Infrastructure.RepositoryImplementations
{
	public class CraftsmanProposalRepository(ApplicationDbContext context) : Repository<CraftsmanProposal>(context), ICraftsmanProposalRepository
	{
		public async Task<IEnumerable<CraftsmanProposal>> GetProposalsByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
		{
			return await _context.CraftsmanProposals
				.Where(p => p.ProjectId == projectId)
				.OrderByDescending(p => p.CoverLetter)
				.ToListAsync(cancellationToken);
		}

		public async Task<IEnumerable<CraftsmanProposal>> GetProposalsByCraftsmanAsync(Guid craftsmanId, CancellationToken cancellationToken = default)
		{
			return await _context.CraftsmanProposals
				.Where(p => p.CraftsmanId == craftsmanId)
				.OrderByDescending(p => p.Status)
				.ToListAsync(cancellationToken);
		}

		public async Task<bool> HasCraftsmanAppliedAsync(Guid projectId, Guid craftsmanId, CancellationToken cancellationToken = default)
		{
			return await _context.CraftsmanProposals
				.AnyAsync(p => p.ProjectId == projectId && p.CraftsmanId == craftsmanId, cancellationToken);
		}

		public async Task<IEnumerable<CraftsmanProposal>> GetPendingProposalsByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
		{
			return await _context.CraftsmanProposals
				.Where(p => p.ProjectId == projectId && p.Status == ProposalStatus.Pending)
				.ToListAsync(cancellationToken);
		}
	}
}