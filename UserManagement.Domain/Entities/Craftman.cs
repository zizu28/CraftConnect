using Core.SharedKernel.Domain;
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

		private List<Skill> _skills = [];
		public IReadOnlyCollection<Skill>? Skills => _skills.AsReadOnly();
		public string ProfileImageUrl { get; set; } = string.Empty;
		public List<Project> Portfolio { get; set; } = [];
		public List<WorkEntry> WorkExperience { get; set; } = [];
		public string Location { get; set; } = string.Empty;
		private Craftman() : base() { }

		public Craftman(Email email, Profession profession) 
			: base(email, UserRole.Craftman) 
		{
			Profession = profession;
		}

		public void AddSkill(string name, int yearsOfExperience)
		{
			_skills.Add(new Skill(name, yearsOfExperience));
			AddIntegrationEvent(new CraftmanSkillIntegrationAddedEvent(Id, name));
		}

		public void VerifyCraftman(Email email, Profession craftmanProfession, IdentityDocument document)
		{
			var craftman = new Craftman(email, craftmanProfession)
			{
				Status = VerificationStatus.Verified,
			};
			craftman.AddIntegrationEvent(new CraftmanVerifiedIntegrationEvent(craftman.Id, document.Type));
		}
	}
}
