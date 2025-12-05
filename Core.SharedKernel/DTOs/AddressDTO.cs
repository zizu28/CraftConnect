namespace Core.SharedKernel.DTOs
{
	public record AddressDTO
	{
		public string Street { get; set; }
		public string City { get; set; }
		public string PostalCode { get; set; }
	}
}
