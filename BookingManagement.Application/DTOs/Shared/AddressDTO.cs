namespace BookingManagement.Application.DTOs.Shared
{
	public record AddressDTO
	{
		public string Street { get; set; }
		public string City { get; set; }
		public string PostalCode { get; set; }
	}
}
