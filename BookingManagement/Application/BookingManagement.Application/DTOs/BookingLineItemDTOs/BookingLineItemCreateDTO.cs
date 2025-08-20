namespace BookingManagement.Application.DTOs.BookingLineItemDTOs
{
	public record BookingLineItemCreateDTO(Guid bookingId, string Description, decimal Price, int Quantity);
	
}
