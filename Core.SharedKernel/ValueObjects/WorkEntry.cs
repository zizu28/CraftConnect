namespace Core.SharedKernel.ValueObjects
{
	public record WorkEntry
	{
		public string Company { get; set; }
		public string Position { get; set; }
		public string Responsibilities { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}
