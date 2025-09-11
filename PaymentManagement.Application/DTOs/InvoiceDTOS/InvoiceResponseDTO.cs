using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using PaymentManagement.Application.DTOs.InvoiceLineItemDTOs;
using PaymentManagement.Application.DTOs.InvoiceRecipientDTOs;
using PaymentManagement.Application.DTOs.PaymentDTOs;

namespace PaymentManagement.Application.DTOs.InvoiceDTOS
{
	public class InvoiceResponseDTO
	{
		public Guid Id { get; set; }
		public string InvoiceNumber { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;
		public Guid IssuedTo { get; set; }
		public Guid IssuedBy { get; set; }
		public Guid? BookingId { get; set; }
		public Guid? OrderId { get; set; }
		public DateTime IssuedDate { get; set; }
		public DateTime DueDate { get; set; }
		public DateTime? PaidDate { get; set; }
		public decimal SubTotal { get; set; }
		public decimal TaxAmount { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal TotalAmount { get; set; }
		public string Currency { get; set; } = string.Empty;
		public decimal TaxRate { get; set; }
		public string? Notes { get; set; }
		public string? Terms { get; set; }
		public InvoiceRecipientResponseDTO Recipient { get; set; } = new();
		public Address? BillingAddress { get; set; }
		public List<InvoiceLineItemResponseDTO> LineItems { get; set; } = [];
		public List<PaymentResponseDTO> Payments { get; set; } = [];
		public bool IsOverdue { get; set; }
		public decimal OutstandingAmount { get; set; }
		public int DaysUntilDue { get; set; }
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public List<string> Errors { get; set; } = [];
	}
}
