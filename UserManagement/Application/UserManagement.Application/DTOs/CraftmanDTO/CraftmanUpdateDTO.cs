namespace UserManagement.Application.DTOs.CraftmanDTO
{
	public class CraftmanUpdateDTO
	{
		public Guid CraftmanId { get; set; }
		public required string Bio { get; set; }
		public required string Status { get; set; }
		public decimal HourlyRate { get; set; }
		public required string Currency { get; set; }
		public required string Profession { get; set; }
		public bool IsAvailable { get; set; }
		public List<string> Skills { get; set; } = [];
	}
}
