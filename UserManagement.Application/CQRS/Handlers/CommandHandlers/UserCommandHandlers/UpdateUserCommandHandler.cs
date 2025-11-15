using Core.Logging;
using Infrastructure.BackgroundJobs;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class UpdateUserCommandHandler(
		IUserRepository userRepository,
		ILoggingService<UpdateUserCommandHandler> logger,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, Unit>
	{
		public Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
