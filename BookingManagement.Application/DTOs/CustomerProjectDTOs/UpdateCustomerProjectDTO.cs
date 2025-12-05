using BookingManagement.Application.DTOs.Shared;

namespace BookingManagement.Application.DTOs.CustomerProjectDTOs
{
	public class UpdateCustomerProjectDTO
	{
		public Guid ProjectId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public MoneyDTO Budget { get; set; } = new();
		public DateTimeRangeDTO Timeline { get; set; } = new();
		public List<SkillDTO> RequiredSkills { get; set; } = [];
	}
}
