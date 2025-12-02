namespace Core.SharedKernel.Domain
{
	public class SkillsDTO(string name, int yearsOfExperience)
	{
		public string Name { get; set; } = name;
		public int YearsOfExperience { get; set; } = yearsOfExperience;
	}
}
