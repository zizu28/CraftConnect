namespace BookingManagement.Application.DTOs.BookingLineItemDTOs
{
	public record BookingLineItemResponseDTO
	{
		public Guid Id { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public bool IsSuccess { get; set; }
		public string Message { get; set; } = string.Empty;
		public List<string> Errors { get; set; } = [];
	}
	
}
