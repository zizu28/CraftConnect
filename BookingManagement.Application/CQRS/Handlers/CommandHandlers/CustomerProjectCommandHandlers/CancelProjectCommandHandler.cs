using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.CustomerProjectCommands;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.CustomerProjectCommandHandlers
{
	public class CancelProjectCommandHandler(ICustomerProjectRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CancelProjectCommand, bool>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<bool> Handle(CancelProjectCommand request, CancellationToken cancellationToken)
		{
			var project = await _repository.GetByIdAsync(request.ProjectId, cancellationToken);
			if (project == null || project.CustomerId != request.CustomerId) return false;

			project.CloseProject();

			await _repository.UpdateAsync(project, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}