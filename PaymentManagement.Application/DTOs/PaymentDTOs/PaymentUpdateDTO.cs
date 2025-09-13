using Core.SharedKernel.Enums;

namespace PaymentManagement.Application.DTOs.PaymentDTOs
{
	public class PaymentUpdateDTO
	{
		public required Guid PaymentId { get; set; }

		public string? Description { get; set; }

		public string? BillingStreet { get; set; }

		public string? BillingCity { get; set; }

		public string? BillingPostalCode { get; set; }

		//public string? BillingState { get; set; }

		//public string? BillingCountry { get; set; }

		public string? Status { get; set; }

		public string? ExternalTransactionId { get; set; }

		public string? PaymentIntentId { get; set; }

		public string? FailureReason { get; set; }
	}
}
