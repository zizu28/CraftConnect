using System.Text.Json.Serialization;
using Core.SharedKernel.Enums;
using Core.SharedKernel.Events;
using Core.SharedKernel.ValueObjects;

namespace UserManagement.Domain.Entities
{
	public class Craftman : User
	{
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Profession Profession { get; private set; }
		public string Bio { get; private set; } = string.Empty;
		public Money HourlyRate { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public VerificationStatus Status { get; private set; } = VerificationStatus.Unverified;
		public bool IsAvailable { get; private set; } = false;

		private List<Skill> _skills = [];
		public IReadOnlyCollection<Skill> Skills => _skills.AsReadOnly();

		private Craftman() : base() { }

		public Craftman(Email email, Profession profession) 
			: base(email, UserRole.Craftman) 
		{
			Profession = profession;
		}

		//public Craftman(Email email, Profession profession, string bio, Money hourlyRate,
		//VerificationStatus status, bool isAvailable, string firstName,
		//string lastName, string userName) : base(email, UserRole.Craftman)
		//{
		//	Profession = profession;
		//	Bio = bio;
		//	HourlyRate = hourlyRate;
		//	Status = status;
		//	IsAvailable = isAvailable;
		//	FirstName = firstName;
		//	LastName = lastName;
		//	Username = userName;
		//}

		public void AddSkill(string name, int yearsOfExperience)
		{
			_skills.Add(new Skill(name, yearsOfExperience));
			AddDomainEvent(new CraftmanSkillAddedEvent(Id, name));
		}

		public static void VerifyCraftman(Email email, Profession craftmanProfession, IdentityDocument document)
		{
			var craftman = new Craftman(email, craftmanProfession)
			{
				Status = VerificationStatus.Verified,
			};
			craftman.AddDomainEvent(new CraftmanVerifiedEvent(craftman.Id, document.Type));
		}
	}
}
