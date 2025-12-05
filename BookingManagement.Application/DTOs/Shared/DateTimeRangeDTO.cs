namespace BookingManagement.Application.DTOs.Shared
{
	public record DateTimeRangeDTO
	{
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}
}
