namespace Core.SharedKernel.DTOs
{
	public class UserResponseDTO
	{
		public Guid UserId { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string ProfileUrl { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public string Message { get; set; } = string.Empty;
		public List<string> Errors { get; set; } = [];
	}

}
