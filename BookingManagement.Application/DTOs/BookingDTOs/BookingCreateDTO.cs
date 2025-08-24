using NodaTime;

namespace BookingManagement.Application.DTOs.BookingDTOs
{
	public record BookingCreateDTO(
		Guid CustomerId, 
		Guid CraftmanId, 
		string Street, 
		string City, 
		string PostalCode,
		string InitialDescription,
		LocalDateTime StartDate,
		LocalDateTime EndDate,
		string Status = "Pending");
}
