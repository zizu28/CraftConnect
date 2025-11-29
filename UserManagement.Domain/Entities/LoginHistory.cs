using Core.SharedKernel.Domain;

namespace UserManagement.Domain.Entities
{
	public class LoginHistory : Entity
	{
		public string IpAddress { get; private set; } = string.Empty;
		public string Device { get; private set; } = string.Empty;
		public string Location { get; private set; } = string.Empty;

		private LoginHistory() { }

		public static LoginHistory Create(string ipAddress, string device, string location)
		{
			return new LoginHistory
			{
				IpAddress = ipAddress,
				Device = device,
				Location = location
			};
		}
	}
}
