namespace Core.SharedKernel.ValueObjects
{
	public record Skill
	{
		public Guid SkillId { get; private set; }
		public string Name { get; set; }
		public int YearsOfExperience { get; set; }
		private Skill()
		{
			SkillId = Guid.Empty;
			Name = string.Empty;
			YearsOfExperience = 0;
		}
		public Skill(string name, int yearsOfExperience)
		{
			if(string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Skill name cannot be null or empty.", nameof(name));
			}
			if(yearsOfExperience < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(yearsOfExperience), "Years of experience cannot be negative.");
			}
			SkillId = Guid.NewGuid();
			Name = name;
			YearsOfExperience = yearsOfExperience;
		}
	}
}
