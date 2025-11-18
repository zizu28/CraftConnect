using Core.SharedKernel.Domain;

namespace UserManagement.Domain.Entities
{
	public class NotificationPreferences : Entity
	{
		public Guid UserId { get; private set; }
		public CraftConnectUser User { get; private set; } = new();
		public bool ReceiveAllNotifications { get; private set; }
		public bool MsgNewMessagesEmail { get; private set; }
		public bool MsgNewMessagesInApp { get; private set; }
		public bool MsgNewMessagesPush { get; private set; }
		public bool ProjProposalAcceptedEmail { get; private set; }
		public bool ProjProposalAcceptedInApp { get; private set; }

		private NotificationPreferences() { }
	}
}
