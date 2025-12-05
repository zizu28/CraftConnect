using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.CustomerProjectCommands;
using Core.SharedKernel.Enums;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.CustomerProjectCommandHandlers
{
	public class UpdateCustomerProjectCommandHandler(
		ICustomerProjectRepository repository,
		IUnitOfWork unitOfWork,
		IMapper mapper) : IRequestHandler<UpdateCustomerProjectCommand, bool>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;

		public async Task<bool> Handle(UpdateCustomerProjectCommand request, CancellationToken cancellationToken)
		{
			var project = await _repository.GetByIdAsync(request.ProjectId, cancellationToken);

			if (project == null) return false;
			if (project.CustomerId != request.CustomerId) return false;
			if (project.Status != ServiceRequestStatus.Open)
				throw new InvalidOperationException("Cannot edit a project that is in progress or closed.");

			project = _mapper.Map(request.Data, project);

			await _repository.UpdateAsync(project, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}