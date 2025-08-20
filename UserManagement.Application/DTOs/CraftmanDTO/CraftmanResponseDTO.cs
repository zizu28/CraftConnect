namespace UserManagement.Application.DTOs.CraftmanDTO
{
	public class CraftmanResponseDTO
	{
		public string FirstName { get; set; }
		public string LastName { get; set; } 
		public string Message { get; set; } = string.Empty;
		public bool IsSuccessful { get; set; } = false;
		public List<string> Errors { get; set; } = [];
		public Guid Id { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Profession { get; set; }
		public string? Bio { get; set; }
		public decimal HourlyRate { get; set; }
		public string? Currency { get; set; }
		public string? Status { get; set; }
		public bool IsAvailable { get; set; }
		public List<string> Skills { get; set; } = [];
	}

}
