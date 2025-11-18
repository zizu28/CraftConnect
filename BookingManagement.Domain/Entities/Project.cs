using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace BookingManagement.Domain.Entities
{
	public class Project : AggregateRoot
	{
		public Guid CustomerId { get; private set; }
		public Guid? SelectedCraftsmanId { get; private set; }
		public Guid? SelectedProposalId { get; private set; }
		public string Title { get; private set; } = string.Empty;
		public string Description { get; private set; } = string.Empty;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public ServiceRequestStatus Status { get; private set; }
		public Money Budget { get; private set; } = new(0, "");
		public DateTimeRange Timeline { get; private set; }
		public List<Skill> Skills { get; private set; } = [];
		public List<Guid> ProposalIds { get; private set; } = [];
		public List<Guid> MilestoneIds { get; private set; } = [];
		public List<Guid> AttachmentIds { get; private set; } = [];

		private Project() { }

		public static Project Post(Guid customerId, string title, string description, Money budget, List<Skill> skills)
		{
			var serviceRequest = new Project
			{
				Id = Guid.NewGuid(),
				CustomerId = customerId,
				Title = title,
				Description = description,
				Budget = budget,
				Skills = skills,
				Status = ServiceRequestStatus.AwaitingProposals
			};
			serviceRequest.AddIntegrationEvent(new ProjectPosted(serviceRequest.Id, customerId, title));
			return serviceRequest;
		}

		public void AddProposal(Guid proposalId)
		{
			ProposalIds.Add(proposalId);
		}

		public void AcceptProposal(Guid proposalId, Guid craftsmanId)
		{
			if(Status == ServiceRequestStatus.AwaitingProposals 
				&& ProposalIds.Contains(Id))
			{
				
				SelectedCraftsmanId = craftsmanId;
				SelectedProposalId = proposalId;
				Status = ServiceRequestStatus.InProgress;
			}
			AddIntegrationEvent(new ProposalAccepted(proposalId, Id, craftsmanId, CustomerId,
					Budget.Amount, Budget.Currency));
		}

		public void CloseProject()
		{
			Status = ServiceRequestStatus.Closed;
		}

		public void AddMilestone(string title, DateTime duedate)
		{
			var milestone = Milestone.Create(title, duedate);
			MilestoneIds.Add(milestone.Id);
		}

		public void AddAttachment(Guid documentId)
		{
			AttachmentIds.Add(documentId);
		}
	}
}
