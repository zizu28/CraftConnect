using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;

namespace UserManagement.Domain.Entities
{
	public class CraftsmanProfileUpdateDTO
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Bio { get; set; } = string.Empty;
		public Profession Profession { get; set; }
		public string ProfileImageUrl { get; set; } = string.Empty;

		public List<SkillsDTO> Skills { get; set; } = [];

		public List<Project> Portfolio { get; set; } = [];
		public List<WorkEntry> WorkExperience { get; set; } = [];

		public string EmailAddress { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
	}

	// Reuse these models for clarity (Portfolio and Work History are likely separate)
	public record Project
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
	}
	public record WorkEntry
	{
		public string Company { get; set; }
		public string Position { get; set; }
		public string Responsibilities { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}
