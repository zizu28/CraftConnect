using Core.SharedKernel.Enums;

namespace PaymentManagement.Application.DTOs.InvoiceRecipientDTOs
{
	public class InvoiceRecipientResponseDTO
	{
		public string Name { get; set; } = string.Empty;
		public string? CompanyName { get; set; }
		public string Email { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
		public string? TaxId { get; set; }
		public string? RegistrationNumber { get; set; }
		public string Type { get; set; } = string.Empty;
		public string DisplayName { get; set; } = string.Empty;
	}
}
