using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.CustomerProjectCommands;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.ValueObjects;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.CustomerProjectCommandHandlers
{
	public class CreateCustomerProjectCommandHandler(
		ICustomerProjectRepository repository,
		IUnitOfWork unitOfWork) : IRequestHandler<CreateCustomerProjectCommand, Guid>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<Guid> Handle(CreateCustomerProjectCommand request, CancellationToken cancellationToken)
		{
			var budget = new Money(request.Data.Budget.Amount, request.Data.Budget.Currency);
			var timeline = new DateTimeRange(request.Data.Timeline.Start, request.Data.Timeline.End);
			var skills = request.Data.RequiredSkills
				.Select(s => new Skill(s.Name, s.YearsOfExperience))
				.ToList();

			var project = CustomerProject.Post(
				request.CustomerId,
				request.Data.Title,
				request.Data.Description,
				budget,
				timeline,
				skills
			);

			foreach (var docId in request.Data.AttachmentIds)
			{
				project.AddAttachment(docId);
			}

			await _repository.AddAsync(project, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return project.Id;
		}
	}
}