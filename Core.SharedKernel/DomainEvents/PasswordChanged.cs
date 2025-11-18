using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record PasswordChanged : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid UserId { get; }
		public string Email { get; }

		public PasswordChanged(Guid userId, string email)
		{
			UserId = userId;
			Email = email;
		}
	}
}
