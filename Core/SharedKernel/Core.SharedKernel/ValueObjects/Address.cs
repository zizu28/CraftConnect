namespace Core.SharedKernel.ValueObjects
{
	public record Address
	{
		public string Street { get; init; } = string.Empty;
		public string City { get; init; } = string.Empty;
		public string PostalCode { get; init; } = string.Empty;

		public Address(string street, string city, string postalCode)
		{
			Street = street;
			City = city;
			PostalCode = postalCode;
		}

		public bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(Street)
				&& !string.IsNullOrWhiteSpace(City)
				&& !string.IsNullOrWhiteSpace(PostalCode);
		}
	}
}
