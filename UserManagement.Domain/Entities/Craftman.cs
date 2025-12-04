using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.AllUserActivitiesIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace UserManagement.Domain.Entities
{
	public class Craftman : User
	{
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Profession Profession { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public VerificationStatus Status { get; private set; } = VerificationStatus.Unverified;
		public string Bio { get; private set; } = string.Empty;
		public List<Skill> Skills { get; private set; } = [];
		public string ProfileImageUrl { get; private set; } = string.Empty;
		public List<Project> Portfolio { get; private set; } = [];
		public List<WorkEntry> WorkExperience { get; private set; } = [];
		public string Location { get; private set; } = string.Empty;
		private Craftman() : base() { }

		public Craftman(Email email, Profession profession) 
			: base(email, UserRole.Craftman) 
		{
			Profession = profession;
		}

		public void AddSkill(string name, int yearsOfExperience)
		{
			Skills.Add(new Skill(name, yearsOfExperience));
			AddIntegrationEvent(new CraftmanSkillIntegrationAddedEvent(Id, name));
		}

		public void VerifyCraftman(IdentityDocument document)
		{
			this.Status = VerificationStatus.Verified;
			AddIntegrationEvent(new CraftmanVerifiedIntegrationEvent(Id, document.Type));
		}
	}
}
