using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.CustomerProjectCommands;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.CustomerProjectCommandHandlers
{
	public class AcceptProposalCommandHandler(
		ICustomerProjectRepository projectRepo,
		ICraftsmanProposalRepository proposalRepo,
		IUnitOfWork unitOfWork) : IRequestHandler<AcceptProposalCommand, bool>
	{
		private readonly ICustomerProjectRepository _projectRepo = projectRepo;
		private readonly ICraftsmanProposalRepository _proposalRepo = proposalRepo;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<bool> Handle(AcceptProposalCommand request, CancellationToken cancellationToken)
		{
			var project = await _projectRepo.GetByIdAsync(request.ProjectId, cancellationToken);
			var proposal = await _proposalRepo.GetByIdAsync(request.ProposalId, cancellationToken);

			if (project == null || proposal == null) return false;

			if (project.CustomerId != request.CustomerId) return false;
			if (proposal.ProjectId != project.Id)
				throw new InvalidOperationException("Proposal does not belong to this project.");

			project.AcceptProposal(proposal.Id, proposal.CraftsmanId, proposal.Price);

			proposal.MarkAsAccepted();

			await _projectRepo.UpdateAsync(project, cancellationToken);
			await _proposalRepo.UpdateAsync(proposal, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			// 6. Optional: Reject other proposals logic here or via Domain Events

			return true;
		}
	}
}