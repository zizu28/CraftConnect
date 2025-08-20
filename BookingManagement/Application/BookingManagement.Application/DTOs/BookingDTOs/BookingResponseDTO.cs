using BookingManagement.Application.DTOs.BookingLineItemDTOs;
using BookingManagement.Application.DTOs.JobDetailsDTOs;

namespace BookingManagement.Application.DTOs.BookingDTOs
{
	public record BookingResponseDTO
	{
		public Guid BookingId { get; set; }
		public Guid CustomerId { get; set; }
		public Guid CraftspersonId { get; set; }
		public string Status { get; set; }
		public JobDetailsResponseDTO Details { get; set; }
		public AddressDTO ServiceAddress { get; set; }
		public IEnumerable<BookingLineItemResponseDTO> LineItems { get; set; }
		public decimal TotalPrice { get; set; }
		public bool IsSuccess { get; set; }
		public string Message { get; set; } = "";
		public List<string> Errors { get; set; } = [];
	}
}
