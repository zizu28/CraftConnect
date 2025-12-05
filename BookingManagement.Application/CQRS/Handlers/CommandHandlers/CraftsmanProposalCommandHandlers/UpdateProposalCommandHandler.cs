using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.CraftsmanProposalCommands;
using Core.SharedKernel.Enums;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.CraftsmanProposalCommandHandlers
{
	public class UpdateProposalCommandHandler(
		ICraftsmanProposalRepository repository,
		IUnitOfWork unitOfWork,
		IMapper mapper) : IRequestHandler<UpdateProposalCommand, bool>
	{
		private readonly ICraftsmanProposalRepository _repository = repository;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;

		public async Task<bool> Handle(UpdateProposalCommand request, CancellationToken cancellationToken)
		{
			var proposal = await _repository.GetByIdAsync(request.ProposalId, cancellationToken);

			if (proposal == null) return false;
			if (proposal.CraftsmanId != request.CraftsmanId) return false;
			if (proposal.Status != ProposalStatus.Pending)
				throw new InvalidOperationException("Can only edit pending proposals.");

			proposal = _mapper.Map(request.Data, proposal);

			await _repository.UpdateAsync(proposal, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}