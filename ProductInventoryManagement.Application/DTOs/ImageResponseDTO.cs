namespace ProductInventoryManagement.Application.DTOs
{
	public record ImageResponseDTO
	{
		public required string Url { get; set; }
		public required string AltText { get; set; }
	}
}
