namespace Core.SharedKernel.ValueObjects
{
	public record UserAddress
	{
		public string Street { get; init; } = string.Empty;
		public string City { get; init; } = string.Empty;
		public string PostalCode { get; init; } = string.Empty;
		public GeoLocation? Location { get; init; } 

		private UserAddress() 
		{
			Street = string.Empty;
			City = string.Empty;
			PostalCode = string.Empty;
			Location = new GeoLocation(0, 0);
		}
		public UserAddress(string street, string city, string postalCode, GeoLocation? location)
		{
			Street = street;
			City = city;
			PostalCode = postalCode;
			Location = location;
		}

		public bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(Street)
				&& !string.IsNullOrWhiteSpace(City)
				&& !string.IsNullOrWhiteSpace(PostalCode);
		}
	}
}
