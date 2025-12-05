using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.CustomerProjectCommands;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.CustomerProjectCommandHandlers
{
	public class RejectProposalCommandHandler(
		ICustomerProjectRepository projectRepo,
		ICraftsmanProposalRepository proposalRepo,
		IUnitOfWork unitOfWork) : IRequestHandler<RejectProposalCommand, bool>
	{
		private readonly ICustomerProjectRepository _projectRepo = projectRepo;
		private readonly ICraftsmanProposalRepository _proposalRepo = proposalRepo;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		public async Task<bool> Handle(RejectProposalCommand request, CancellationToken cancellationToken)
		{
			var project = await _projectRepo.GetByIdAsync(request.ProjectId, cancellationToken);
			var proposal = await _proposalRepo.GetByIdAsync(request.ProposalId, cancellationToken);
			if (project == null || proposal == null) return false;
			if (project.CustomerId != request.CustomerId) return false;
			if (proposal.ProjectId != project.Id)
				throw new InvalidOperationException("Proposal does not belong to this project.");
			proposal.MarkAsRejected();
			await _proposalRepo.UpdateAsync(proposal, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}