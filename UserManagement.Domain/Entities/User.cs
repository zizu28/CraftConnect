using Core.SharedKernel.Domain;
using System.Text.Json.Serialization;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.ComponentModel.DataAnnotations;
using Core.SharedKernel.IntegrationEvents.AllUserActivitiesIntegrationEvents;

namespace UserManagement.Domain.Entities
{
	public class User : AggregateRoot
	{
		public string Username { get; private set; } = string.Empty;
		public string FirstName { get; private set; } = string.Empty;
		public string LastName { get; private set; } = string.Empty;
		public bool AgreeToTerms { get; private set; }
		public Email Email { get; private set; }
		public bool IsEmailConfirmed { get; set; } = false;
		public string PasswordHash { get; set; } = string.Empty;
		public PhoneNumber Phone { get; private set; } = new("", "");
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public UserRole Role { get; private set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		[Timestamp]
		public byte[] RowVersion { get; set; }
		public List<RefreshToken> RefreshTokens { get; set; } = [];
		public List<EmailVerificationToken> EmailVerificationTokens { get; set; } = [];

		public User() {}
		public User(Email email, UserRole role)
		{
			Id = Guid.NewGuid();
			Email = email;
			Role = role;
			CreatedAt = DateTime.UtcNow;
			AddIntegrationEvent(new UserRegisteredIntegrationEvent(Id, email, role));
		}
	}
}
