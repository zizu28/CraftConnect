namespace Core.SharedKernel.ValueObjects
{
	public record Skill
	{
		public string Name { get; private set; }
		public int YearsOfExperience { get; private set; }
		private Skill()
		{
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
			Name = name;
			YearsOfExperience = yearsOfExperience;
		}
	}
}
