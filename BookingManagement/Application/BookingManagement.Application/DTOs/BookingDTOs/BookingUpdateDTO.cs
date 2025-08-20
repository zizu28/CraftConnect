namespace BookingManagement.Application.DTOs.BookingDTOs
{
	public record BookingUpdateDTO(
	Guid BookingId,
	string NewDescription,
	string Status,
	string Street, 
	string City, 
	string PostalCode);
}
