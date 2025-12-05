namespace Core.SharedKernel.DTOs
{
	public record BookingLineItemCreateDTO(Guid BookingId, string Description, decimal Price, int Quantity);
	
}
