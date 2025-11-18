using Core.SharedKernel.Domain;

namespace UserManagement.Domain.Entities
{
	public class LinkedDevice : Entity
	{
		public string Device { get; private set; } = string.Empty;
		public string Location { get; private set; } = string.Empty;
		public DateTime LastSeen { get; private set; }
		public bool IsActive { get; private set; }

		private LinkedDevice() { }

		public static LinkedDevice Create(string device, string location, string deviceId)
		{
			return new LinkedDevice
			{
				Device = device,
				Location = location, 
				Id = Guid.Parse(deviceId)
			};
		}
	}
}
