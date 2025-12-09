using Core.SharedKernel.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class ProjectMilestone
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public DateTime DueDate { get; set; }
		public MilestoneStatus Status { get; set; }
		public PaymentStatus PaymentStatus { get; set; }
	}
}
