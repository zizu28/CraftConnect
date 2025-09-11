using System.ComponentModel.DataAnnotations;

namespace PaymentManagement.Application.DTOs.RefundDTOs
{
	public class RefundCreateDTO
	{
		public required Guid PaymentId { get; set; }

		public required decimal Amount { get; set; }

		public required string Reason { get; set; } = string.Empty;

		public required Guid InitiatedBy { get; set; }
	}
}
