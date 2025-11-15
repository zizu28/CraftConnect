using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace UserManagement.Domain.Entities
{
	public class CraftsmanProfileUpdateDTO
	{
		public Guid CraftmanId { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Bio { get; set; } = string.Empty;
		public string Profession { get; set; } = string.Empty;
		public string VerificationStatus { get; set; } = string.Empty;
		public string ProfileImageUrl { get; set; } = string.Empty;

		public List<SkillsDTO> Skills { get; set; } = [];

		public List<Project> Portfolio { get; set; } = [];
		public List<WorkEntry> WorkExperience { get; set; } = [];

		public string EmailAddress { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
	}
}
