namespace Core.SharedKernel.DTOs
{
	public record DateTimeRangeDTO
	{
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}
}
