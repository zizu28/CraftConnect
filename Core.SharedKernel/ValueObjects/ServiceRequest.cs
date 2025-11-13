using Core.SharedKernel.Enums;

namespace Core.SharedKernel.ValueObjects
{
	public record ServiceRequest
	{
		public string Title { get; private set; } = string.Empty;
		public string Description { get; private set; } = string.Empty;
		public Guid Id { get; set; }
		public string ProjectTitle { get; private set; } = string.Empty;
		public string Category { get; private set; } = string.Empty;
		public DateTime DateSubmitted { get; private set; }
		public ServiceRequestStatus Status { get; private set; }
		public string ProjectDescription { get; private set; }
		public List<string> SkillsRequired { get; private set; } = [];
		public decimal Budget { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }
		public List<AttachedDocument> Attachments { get; private set; } = [];
		public List<Proposal> Proposals { get; set; } = [];

		public ServiceRequest()
		{
			
		}
	}
}
