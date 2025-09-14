namespace BookingManagement.Application.DTOs.BookingLineItemDTOs
{
	public record BookingLineItemCreateDTO(Guid BookingId, string Description, decimal Price, int Quantity);
	
}
