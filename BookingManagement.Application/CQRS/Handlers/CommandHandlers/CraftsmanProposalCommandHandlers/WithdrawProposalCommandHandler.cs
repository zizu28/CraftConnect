using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.CraftsmanProposalCommands;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.CraftsmanProposalCommandHandlers
{
	public class WithdrawProposalCommandHandler(ICraftsmanProposalRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<WithdrawProposalCommand, bool>
	{
		private readonly ICraftsmanProposalRepository _repository = repository;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<bool> Handle(WithdrawProposalCommand request, CancellationToken cancellationToken)
		{
			var proposal = await _repository.GetByIdAsync(request.ProposalId, cancellationToken);

			if (proposal == null) return false;
			if (proposal.CraftsmanId != request.CraftsmanId) return false;

			proposal.Withdraw();

			await _repository.UpdateAsync(proposal, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}