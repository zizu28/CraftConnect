using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace UserManagement.Domain.Entities
{
	public class CraftConnectUser : AggregateRoot
	{
		public Email Email { get; private set; } = new("");
		public string PasswordHash { get; private set; } = string.Empty;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public UserRole UserRole { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public UserStatus UserStatus { get; private set; }
		public bool IsTwoFactorEnabled { get; private set; }
		public List<LoginHistory> LoginHistories { get; private set; } = [];
		public List<LinkedDevice> LinkedDevices { get; private set; } = [];
		public Guid NotificationPreferencesId { get; private set; }

		public static CraftConnectUser Register(string email, string passwordHash, UserRole role)
		{
			var user = new CraftConnectUser
			{
				PasswordHash = passwordHash,
				UserRole = role,
				UserStatus = UserStatus.EmailNotVerified
			};
			user.AddIntegrationEvent(new NewUserRegistered(user.Id, email, role.ToString()));
			return user;
		}

		public void VerifyEmail()
		{
			if(UserStatus == UserStatus.EmailNotVerified)
			{
				UserStatus = UserStatus.Active;
			}
		}

		public bool ChangePassword(string oldPasswordHash, string newPasswordHash)
		{
			if(PasswordHash == oldPasswordHash)
			{
				PasswordHash = newPasswordHash;
				//AddLoginHistory();
				AddIntegrationEvent(new PasswordChanged(Id, Email.Address));
				return true;
			}
			return false;
		}

		public void SuspendUser()
		{
			UserStatus = UserStatus.Suspended;
		}

		public void DeactivateUser()
		{
			UserStatus = UserStatus.Deactivated;
			AddIntegrationEvent(new UserDeactivated(Id));
		}

		public void EnableTwoFactor()
		{
			IsTwoFactorEnabled = true;
		}

		public void AddLoginHistory(string ipAddress, string device, string location)
		{
			var loginHistory = LoginHistory.Create(ipAddress, device, location);
			LoginHistories.Add(loginHistory);
		}

		public void AddLinkedDevice(string device, string location, string deviceId)
		{
			var linkedDevice = LinkedDevice.Create(device, location, deviceId);
			LinkedDevices.Add(linkedDevice);
		}

		public void RemoveLinkedDevice(Guid linkedDeviceId)
		{
			var linkedDevice = LinkedDevices.FirstOrDefault(device => device.Id == linkedDeviceId);
			if(linkedDevice != null) LinkedDevices.Remove(linkedDevice);			
		}
	}
}
