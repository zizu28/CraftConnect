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
		public string Username { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Email Email { get; private set; }
		public bool IsEmailConfirmed { get; set; } = false;
		public string PasswordHash { get; set; }
		public PhoneNumber Phone { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public UserRole Role { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		[Timestamp]
		public byte[] RowVersion { get; set; }
		public List<RefreshToken> RefreshTokens { get; set; } = [];

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
