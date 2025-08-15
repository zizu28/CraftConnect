namespace BookingManagement.Application.DTOs.BookingLineItemDTOs
{
	public record BookingLineItemCreateDTO(string Description, decimal Price, int Quantity);
	
}
