using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace Core.SharedKernel.DTOs
{
	public class CraftmanResponseDTO
	{
		//[JsonPropertyName("Id")]
		public Guid CraftmanId { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public DateTime CreatedAtUtc { get; set; }
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
		public string Message { get; set; } = string.Empty;
		public List<string> Errors { get; set; } = [];
		public bool IsSuccessful { get; set; }
	}

}
