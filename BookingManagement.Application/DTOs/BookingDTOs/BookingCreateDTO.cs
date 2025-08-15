namespace BookingManagement.Application.DTOs.BookingDTOs
{
	public record BookingCreateDTO(
		Guid CustomerId, 
		Guid CraftmanId, 
		string Street, 
		string City, 
		string PostalCode,
		string InitialDescription,
		DateTime StartDate,
		DateTime EndDate,
		string Status = "Pending");
}
