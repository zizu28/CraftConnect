using System.Text.Json.Serialization;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.AllUserActivitiesIntegrationEvents;
using Core.SharedKernel.ValueObjects;

namespace UserManagement.Domain.Entities
{
	public class Craftman : User
	{
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Profession Profession { get; private set; }
		public string Bio { get; private set; } = string.Empty;
		public Money? HourlyRate { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public VerificationStatus Status { get; private set; } = VerificationStatus.Unverified;
		public bool IsAvailable { get; private set; } = false;

		private List<Skill> _skills = [];
		public IReadOnlyCollection<Skill>? Skills => _skills.AsReadOnly();

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
