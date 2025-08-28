using MediatR;

namespace UserManagement.Application.CQRS.Commands.CraftmanCommands
{
	public class AddSkillCommand : IRequest<Unit>
	{
		public Guid CraftmanId { get; set; }
		public string NameOfSkill { get; set; }
		public int YearsOfExperience { get; set; }
	}
}
