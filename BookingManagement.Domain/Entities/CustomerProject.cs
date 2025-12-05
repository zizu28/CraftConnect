using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace BookingManagement.Domain.Entities
{
	public class CustomerProject : AggregateRoot
	{
		public Guid CustomerId { get; private set; }
		public Guid? SelectedCraftsmanId { get; private set; }
		public Guid? SelectedProposalId { get; private set; }
		public string Title { get; private set; } = string.Empty;
		public string Description { get; private set; } = string.Empty;
		public ServiceRequestStatus Status { get; private set; }
		public Money Budget { get; private set; } = new(0, "");
		public DateTimeRange Timeline { get; private set; }
		public List<Skill> Skills { get; private set; } = [];
		public List<Guid> MilestoneIds { get; private set; } = [];
		public List<Guid> AttachmentIds { get; private set; } = [];

		private CustomerProject() { }

		public static CustomerProject Post(Guid customerId, string title, string description, Money budget, DateTimeRange timeline, List<Skill> skills)
		{
			if (budget.Amount < 0) throw new ArgumentException("Budget cannot be negative");

			var project = new CustomerProject
			{
				Id = Guid.NewGuid(),
				CustomerId = customerId,
				Title = title,
				Description = description,
				Budget = budget,
				Timeline = timeline,
				Skills = skills,
				Status = ServiceRequestStatus.Open
			};

			project.AddIntegrationEvent(new ProjectPosted(project.Id, customerId, title));
			return project;
		}

		public void AcceptProposal(Guid proposalId, Guid craftsmanId, Money agreedPrice)
		{
			if (Status != ServiceRequestStatus.Open)
			{
				throw new InvalidOperationException($"Cannot accept proposal. Project status is {Status}.");
			}

			SelectedCraftsmanId = craftsmanId;
			SelectedProposalId = proposalId;
			Status = ServiceRequestStatus.InProgress;

			AddIntegrationEvent(new ProposalAccepted(
				proposalId,
				Id,
				craftsmanId,
				CustomerId,
				agreedPrice.Amount,
				agreedPrice.Currency));
		}

		public void CancelSelection()
		{
			if (Status != ServiceRequestStatus.InProgress) return;

			SelectedCraftsmanId = null;
			SelectedProposalId = null;
			Status = ServiceRequestStatus.Open;
		}

		public void CompleteProject()
		{
			if (Status != ServiceRequestStatus.InProgress) throw new InvalidOperationException("Project is not in progress.");
			Status = ServiceRequestStatus.Completed;
			// Raise Event: ProjectCompleted (Trigger payments, etc)
		}
	}
}