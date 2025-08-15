namespace UserManagement.Application.DTOs.CustomerDTO
{
	public class CustomerCreateDTO
	{
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required string UserName { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string PhoneNumber { get; set; }
		public required string PhoneCountryCode { get; set; }
		public required string Street { get; set; }
		public required string City { get; set; }
		public required string PostalCode { get; set; }
		public string Latitude { get; set; } = "0.0";
		public string Longitude { get; set; } = "0.0";
		public string PreferredPaymentMethod { get; set; } = "Cash";
	}
}
