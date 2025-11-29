using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record NewUserRegistered : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid UserId { get; }
		public string Email { get; }
		public string Role { get; }

		public NewUserRegistered(Guid userId, string email, string role)
		{
			UserId = userId;
			Email = email;
			Role = role;
		}
	}
}
