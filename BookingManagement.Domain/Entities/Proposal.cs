using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookingManagement.Domain.Entities
{
	public class Proposal : AggregateRoot
	{
		public Guid ProjectId { get; private set; }
		public Guid CraftsmanId { get; private set; }
		[Column(TypeName = "decimal(18, 2)")]
		public decimal QuoteSummary { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public VerificationStatus Status { get; private set; }
		public Money Price { get; private set; } = new(0, "");
		public DateTimeRange Timeline { get; private set; } = default!;

		private Proposal() { }

		public static Proposal Submit(Guid projectId, Guid craftsmanId, Money price, decimal summary, DateTimeRange timeline)
		{
			var proposal = new Proposal
			{
				ProjectId = projectId,
				CraftsmanId = craftsmanId,
				Price = price,
				QuoteSummary = summary,
				Timeline = timeline,
				Status = VerificationStatus.Pending
			};
			proposal.AddIntegrationEvent(new ProposalSubmitted(proposal.Id, projectId, craftsmanId));
			return proposal;
		}

		public void Withdraw()
		{
			Status = VerificationStatus.Withdrawn;
		}

		public void Reject()
		{
			Status = VerificationStatus.Rejected;
		}

		public void MarkAsAccepted()
		{
			Status = VerificationStatus.Accepted;
		}
	}
}
