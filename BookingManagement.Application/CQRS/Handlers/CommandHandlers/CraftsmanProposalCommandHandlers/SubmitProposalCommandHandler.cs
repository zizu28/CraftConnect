using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.CraftsmanProposalCommands;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.CraftsmanProposalCommandHandlers
{
	public class SubmitProposalCommandHandler(
		ICraftsmanProposalRepository proposalRepo,
		ICustomerProjectRepository projectRepo,
		IUnitOfWork unitOfWork) : IRequestHandler<SubmitProposalCommand, Guid>
	{
		private readonly ICraftsmanProposalRepository _proposalRepo = proposalRepo;
		private readonly ICustomerProjectRepository _projectRepo = projectRepo;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<Guid> Handle(SubmitProposalCommand request, CancellationToken cancellationToken)
		{
			var project = await _projectRepo.GetByIdAsync(request.Data.ProjectId, cancellationToken) ?? throw new KeyNotFoundException("Project not found.");

			if (project.Status != ServiceRequestStatus.Open) throw new InvalidOperationException("Cannot submit proposal to a project that is not open.");

			var exists = await _proposalRepo.HasCraftsmanAppliedAsync(request.Data.ProjectId, request.CraftsmanId, cancellationToken);
			if (exists) throw new InvalidOperationException("You have already submitted a proposal for this project.");

			var price = new Money(request.Data.Price.Amount, request.Data.Price.Currency);
			var timeline = new DateTimeRange(request.Data.ProposedTimeline.Start, request.Data.ProposedTimeline.End);

			var proposal = CraftsmanProposal.Submit(
				request.Data.ProjectId,
				request.CraftsmanId,
				price,
				request.Data.CoverLetter,
				timeline
			);

			await _proposalRepo.AddAsync(proposal, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return proposal.Id;
		}
	}
}