using Core.SharedKernel.Domain;

namespace ContentAndSkillsManagement.Domain.Entities
{
	public class Category : AggregateRoot
	{
		public string Name { get; private set; } = string.Empty;
		public string IconSvgPath { get; private set; } = string.Empty;
		private List<Guid> SkillsIds { get; set; } = [];

		private Category() { }

		public static Category Create(string name, string icon)
		{
			ArgumentNullException.ThrowIfNullOrEmpty(name);
			ArgumentNullException.ThrowIfNullOrEmpty(icon);
			Category category = new()
			{
				Name = name,
				IconSvgPath = icon,
			};
			return category;
		}

		public void Update(string name, string icon)
		{
			ArgumentException.ThrowIfNullOrEmpty(name);
			ArgumentException.ThrowIfNullOrEmpty(icon);
			Name = name;
			IconSvgPath = icon;
		}

		public void AddSkill(Guid skillId)
		{
			SkillsIds.Add(skillId);
		}

		public void RemoveSkill(Guid skillId)
		{
			SkillsIds.Remove(skillId);
		}
	}
}
