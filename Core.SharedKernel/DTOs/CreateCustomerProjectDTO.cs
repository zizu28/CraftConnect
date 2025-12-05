namespace Core.SharedKernel.DTOs
{
	public class CreateCustomerProjectDTO
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public MoneyDTO Budget { get; set; } = new();
		public DateTimeRangeDTO Timeline { get; set; } = new();
		public List<SkillDTO> RequiredSkills { get; set; } = [];
		public List<Guid> AttachmentIds { get; set; } = [];
	}
}
