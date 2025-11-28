namespace Core.SharedKernel.ValueObjects
{
	public record WorkEntry
	{
		public string Company { get; set; }
		public string Position { get; set; }
		public string Responsibilities { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		private WorkEntry()
		{
			Company = string.Empty;
			Position = string.Empty;
			Responsibilities = string.Empty;
			StartDate = DateTime.MinValue;
			EndDate = DateTime.MinValue;
		}

		public WorkEntry(string company, string position, string responsibilities,
			DateTime startDate, DateTime endDate)
		{
			ArgumentException.ThrowIfNullOrEmpty(company);
			ArgumentException.ThrowIfNullOrEmpty(position);
			ArgumentException.ThrowIfNullOrEmpty(responsibilities);
			if (startDate > endDate) throw new Exception("End date must be greater than start date");
			Company = company;
			Position = position;
			Responsibilities = responsibilities;
			StartDate = startDate;
			EndDate = endDate;
		}
	}
}
