namespace CraftConnect.WebUI.ViewModels
{
	public class NotificationSettingsViewModel
	{
		public bool ReceiveAllNotifications { get; set; } = true;
		public bool MsgNewMessagesEmail { get; set; } = true;
		public bool MsgNewMessagesInApp { get; set; } = true;
		public bool MsgNewMessagesPush { get; set; } = false;
		public bool ProjProposalAcceptedEmail { get; set; } = true;
		public bool ProjProposalAcceptedInApp { get; set; } = true;
		public bool ProjProposalAcceptedPush { get; set; } = true;
		public bool ProjProjectMilestonesEmail { get; set; } = false;
		public bool ProjProjectMilestonesInApp { get; set; } = true;
		public bool ProjProjectMilestonesPush { get; set; } = false;
		public bool SvcNewRequestMatchingSkillsEmail { get; set; } = true;
		public bool SvcNewRequestMatchingSkillsInApp { get; set; } = true;
		public bool SvcNewRequestMatchingSkillsPush { get; set; } = true;
		public bool PmtPaymentConfirmationEmail { get; set; } = true;
		public bool PmtPaymentConfirmationInApp { get; set; } = true;
		public bool PmtPaymentConfirmationPush { get; set; } = false;
	}
}
