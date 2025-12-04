using System.ComponentModel.DataAnnotations;

namespace Core.SharedKernel.ValueObjects
{
	public record WorkEntry
	{
		public Guid WorkEntryId { get; private set; }
		public string Company { get; set; }
		public string Position { get; set; }
		public string Responsibilities { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		[Timestamp]
		public byte[]? RowVersion { get; set; }

		public WorkEntry()
		{
			WorkEntryId = Guid.Empty;
			Company = string.Empty;
			Position = string.Empty;
			Responsibilities = string.Empty;
			StartDate = DateTime.MinValue;
			EndDate = DateTime.MinValue;
		}

		public WorkEntry(string company, string position, string responsibilities,
			DateTime startDate, DateTime endDate)
		{
			WorkEntryId = Guid.NewGuid();
			Company = company;
			Position = position;
			Responsibilities = responsibilities;
			StartDate = startDate;
			EndDate = endDate;
		}
	}
}
