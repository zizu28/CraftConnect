using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using System.Text.Json.Serialization;

namespace BookingManagement.Domain.Entities
{
	public class Milestone : AggregateRoot
	{
		public Guid ProjectId { get; private set; }
		public string Title { get; private set; } = string.Empty;
		public DateTime DueDate { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public MilestoneStatus MilestoneStatus { get; private set; }
		public PaymentStatus PaymentStatus { get; private set; }

		private Milestone() { }

		public static Milestone Create(string title, DateTime duedate)
		{
			return new Milestone
			{
				Title = title,
				DueDate = duedate,
			};
		}

		public void UpdateStatus(MilestoneStatus newStatus)
		{
			if(MilestoneStatus == MilestoneStatus.InProgress)
			{
				MilestoneStatus = newStatus;
			}
		}

		public void MarkAsPaid()
		{
			PaymentStatus = PaymentStatus.Paid;
			AddIntegrationEvent(new PaymentCompleted());
		}
	}
}
