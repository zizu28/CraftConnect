using Core.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.SharedKernel.DTOs
{
	public class InvoiceUpdateDTO
	{
		public required Guid InvoiceId { get; set; }

		public DateTime DueDate { get; set; }

		public decimal? TaxRate { get; set; }

		public string? Notes { get; set; }

		public string? Terms { get; set; }

		public string? BillingStreet { get; set; }

		public string? BillingCity { get; set; }

		public string? BillingPostalCode { get; set; }

		public string? BillingState { get; set; }

		public string? BillingCountry { get; set; }

		public string Status { get; set; } = string.Empty;
	}
}
