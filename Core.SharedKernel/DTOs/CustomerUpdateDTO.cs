namespace Core.SharedKernel.DTOs
{
	public class CustomerUpdateDTO
	{
		public Guid CustomerId { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string Street { get; set; }
		public required string City { get; set; }
		public required string PostalCode { get; set; }
		public required string PreferredPaymentMethod { get; set; }
	}

}
