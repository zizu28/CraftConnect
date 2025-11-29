using Core.SharedKernel.Domain;

namespace ContentAndSkillsManagement.Domain.Entities
{
	public class Skill : AggregateRoot
	{
		public Guid CategoryId { get; private set; }
		public string Name { get; private set; } = string.Empty;
		public string IconSvgPath { get; private set; } = string.Empty;

		private Skill() { }

		public static Skill Create(Guid categoryId, string name, string icon)
		{
			return new Skill
			{
				CategoryId = categoryId,
				Name = name,
				IconSvgPath = icon
			};
		}

		public void Update(string name, string icon)
		{
			Name = name;
			IconSvgPath = icon;
		}
	}
}
