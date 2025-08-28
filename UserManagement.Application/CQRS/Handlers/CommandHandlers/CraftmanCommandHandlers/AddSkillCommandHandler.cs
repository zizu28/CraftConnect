using Core.Logging;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.CraftmanCommands;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.CraftmanCommandHandlers
{
	public class AddSkillCommandHandler(
		ICraftsmanRepository craftsmanRepository,
		ILoggingService<AddSkillCommandHandler> logger) : IRequestHandler<AddSkillCommand, Unit>
	{
		public async Task<Unit> Handle(AddSkillCommand request, CancellationToken cancellationToken)
		{
			if(string.IsNullOrWhiteSpace(request.NameOfSkill))
			{
				logger.LogWarning("Skill name is null or empty.");
				throw new ArgumentException("Skill name cannot be null or empty.");
			}

			var craftman = await craftsmanRepository.GetByIdAsync(request.CraftmanId, cancellationToken)
				?? throw new KeyNotFoundException("Craftman with the specified skill not found.");
			craftman.AddSkill(request.NameOfSkill, request.YearsOfExperience);
			await craftsmanRepository.UpdateAsync(craftman, cancellationToken);
			return Unit.Value;
		}
	}
}
