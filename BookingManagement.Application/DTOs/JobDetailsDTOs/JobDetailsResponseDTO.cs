namespace BookingManagement.Application.DTOs.JobDetailsDTOs
{
	public record JobDetailsResponseDTO
	{
		public Guid BookingId { get; set; }
		public string Description { get; set; }
		public string Message { get; set; }
		public bool IsSuccess { get; set; }
		public List<string> Errors { get; set; } = [];
	}
}
