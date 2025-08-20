using Core.SharedKernel.Domain;
using UserManagement.Domain.Entities;

namespace UserManagement.Domain.UserIntegrationEvents
{
	public record UserCreatedIntegrationEvent(User User) : IIntegrationEvent<User>
	{
		private readonly User _user = User;
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;

		public User Data => _user;
	}
}
