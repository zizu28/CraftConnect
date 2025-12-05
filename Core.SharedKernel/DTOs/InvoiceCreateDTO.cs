using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs
{
	public class InvoiceCreateDTO
	{
		public required string InvoiceType { get; set; }

		public required Guid IssuedTo { get; set; }

		public required Guid IssuedBy { get; set; }

		public decimal SubTotal { get; set; } = 0;
		public decimal TaxAmount { get; set; } = 0;
		public decimal DiscountAmount { get; set; } = 0;
		public decimal TotalAmount { get; set; } = 0;

		public Guid BookingId { get; set; }
		public Guid? OrderId { get; set; }

		public DateTime DueDate { get; set; }

		public string InvoiceStatus { get; set; } = string.Empty;
		public required string Currency { get; set; } = string.Empty;

		public decimal TaxRate { get; set; } = 0;

		public string? Notes { get; set; }

		public string? Terms { get; set; }

		public required string RecipientName { get; set; } = string.Empty;

		public string? RecipientCompanyName { get; set; }

		public required string RecipientEmail { get; set; } = string.Empty;

		public string? RecipientPhone { get; set; }

		public string RecipientType { get; set; } = string.Empty;

		public string? RecipientTaxId { get; set; }

		public string? RecipientRegistrationNumber { get; set; }
		public string? RecipientCountryCode { get; set; }

		public required string BillingStreet { get; set; } = string.Empty;

		public required string BillingCity { get; set; } = string.Empty;

		public required string BillingPostalCode { get; set; } = string.Empty;

		//public string? BillingState { get; set; }

		//public string? BillingCountry { get; set; }

		public List<InvoiceLineItemCreateDTO> LineItems { get; set; } = [];
	}
}
