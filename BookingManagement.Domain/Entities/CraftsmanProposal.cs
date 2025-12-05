using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace BookingManagement.Domain.Entities
{
	public class CraftsmanProposal : AggregateRoot
	{
		public Guid ProjectId { get; private set; }
		public Guid CraftsmanId { get; private set; }
		public string CoverLetter { get; private set; } = string.Empty;
		public ProposalStatus Status { get; private set; }
		public Money Price { get; private set; } = new(0, "");
		public DateTimeRange ProposedTimeline { get; private set; } = default!;
		private CraftsmanProposal() { }

		public static CraftsmanProposal Submit(Guid projectId, Guid craftsmanId, Money price, string coverLetter, DateTimeRange timeline)
		{
			var proposal = new CraftsmanProposal
			{
				Id = Guid.NewGuid(),
				ProjectId = projectId,
				CraftsmanId = craftsmanId,
				Price = price,
				CoverLetter = coverLetter,
				ProposedTimeline = timeline,
				Status = ProposalStatus.Pending
			};

			proposal.AddIntegrationEvent(new ProposalSubmitted(proposal.Id, projectId, craftsmanId));
			return proposal;
		}

		public void Withdraw()
		{
			if (Status == ProposalStatus.Accepted)
				throw new InvalidOperationException("Cannot withdraw an accepted proposal.");

			Status = ProposalStatus.Withdrawn;
		}

		public void MarkAsAccepted()
		{
			Status = ProposalStatus.Accepted;
		}

		public void MarkAsRejected()
		{
			if (Status != ProposalStatus.Accepted)
			{
				Status = ProposalStatus.Rejected;
			}
		}
	}
}