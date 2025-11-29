using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.InvoiceIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PaymentManagement.Domain.Entities
{
	public class Invoice : AggregateRoot
	{
		public string InvoiceNumber { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public InvoiceStatus Status { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public InvoiceType Type { get; private set; }
		public Guid IssuedTo { get; private set; }
		public Guid IssuedBy { get; private set; }
		public Guid? BookingId { get; private set; }
		public Guid? OrderId { get; private set; }
		public DateTime IssuedDate { get; private set; }
		public DateTime DueDate { get; private set; }
		public DateTime? PaidDate { get; private set; }
		public Money SubTotal { get; private set; }
		public Money TaxAmount { get; private set; }
		public Money DiscountAmount { get; private set; }
		public Money TotalAmount { get; private set; }
		public string Currency { get; private set; }
		[Column(TypeName = "decimal(18, 2)")]
		public decimal TaxRate { get; private set; }
		public string? Notes { get; private set; }
		public string? Terms { get; private set; }
		public InvoiceRecipient Recipient { get; private set; }
		public Address BillingAddress { get; private set; }

		[Timestamp]
		public byte[] RowVersion { get; set; }

		private readonly List<InvoiceLineItem> _lineItems = [];
		public IReadOnlyCollection<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

		private readonly List<Payment> _payments = [];
		public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

		private Invoice() { }

		public static Invoice CreateForBooking(
			Guid bookingId,
			Guid issuedTo,
			Guid issuedBy,
			InvoiceRecipient recipient,
			Address billingAddress,
			string currency,
			decimal taxRate = 0,
			DateTime? dueDate = null,
			string? notes = null,
			string? terms = null)
		{
			ValidateInvoiceCreation(issuedTo, issuedBy, recipient, billingAddress, currency);

			var invoice = new Invoice
			{
				Id = Guid.NewGuid(),
				InvoiceNumber = GenerateInvoiceNumber(),
				BookingId = bookingId,
				Type = InvoiceType.Booking,
				IssuedTo = issuedTo,
				IssuedBy = issuedBy,
				Recipient = recipient,
				BillingAddress = billingAddress,
				Status = InvoiceStatus.Draft,
				IssuedDate = DateTime.UtcNow,
				DueDate = dueDate ?? DateTime.UtcNow.AddDays(30),
				Currency = currency,
				TaxRate = taxRate,
				Notes = notes,
				Terms = terms,
				SubTotal = new Money(0, currency),
				TaxAmount = new Money(0, currency),
				DiscountAmount = new Money(0, currency),
				TotalAmount = new Money(0, currency)
			};

			return invoice;
		}

		public static Invoice CreateForOrder(
			Guid orderId,
			Guid issuedTo,
			Guid issuedBy,
			InvoiceRecipient recipient,
			Address billingAddress,
			string currency,
			decimal taxRate = 0,
			DateTime? dueDate = null,
			string? notes = null,
			string? terms = null)
		{
			ValidateInvoiceCreation(issuedTo, issuedBy, recipient, billingAddress, currency);

			var invoice = new Invoice
			{
				Id = Guid.NewGuid(),
				InvoiceNumber = GenerateInvoiceNumber(),
				OrderId = orderId,
				Type = InvoiceType.Order,
				IssuedTo = issuedTo,
				IssuedBy = issuedBy,
				Recipient = recipient,
				BillingAddress = billingAddress,
				Status = InvoiceStatus.Draft,
				IssuedDate = DateTime.UtcNow,
				DueDate = dueDate ?? DateTime.UtcNow.AddDays(30),
				Currency = currency,
				TaxRate = taxRate,
				Notes = notes,
				Terms = terms,
				SubTotal = new Money(0, currency),
				TaxAmount = new Money(0, currency),
				DiscountAmount = new Money(0, currency),
				TotalAmount = new Money(0, currency)
			};

			return invoice;
		}

		public void AddLineItem(string description, decimal unitPrice, int quantity, string? itemCode = null)
		{
			if (Status != InvoiceStatus.Draft)
				throw new InvalidOperationException($"Cannot add line items to invoice in {Status} status");

			if (string.IsNullOrWhiteSpace(description))
				throw new ArgumentException("Line item description cannot be empty");

			if (unitPrice < 0)
				throw new ArgumentException("Unit price cannot be negative");

			if (quantity <= 0)
				throw new ArgumentException("Quantity must be greater than zero");

			var lineItem = InvoiceLineItem.Create(
				Id, description, new Money(unitPrice, Currency), quantity, itemCode);

			_lineItems.Add(lineItem);
			RecalculateTotals();
		}

		public void RemoveLineItem(Guid lineItemId)
		{
			if (Status != InvoiceStatus.Draft)
				throw new InvalidOperationException($"Cannot remove line items from invoice in {Status} status");

			var lineItem = _lineItems.FirstOrDefault(li => li.Id == lineItemId)
				?? throw new ArgumentException($"Line item with ID {lineItemId} not found");

			_lineItems.Remove(lineItem);
			RecalculateTotals();
		}

		public void UpdateLineItem(Guid lineItemId, string? description = null, decimal? unitPrice = null, int? quantity = null)
		{
			if (Status != InvoiceStatus.Draft)
				throw new InvalidOperationException($"Cannot update line items on invoice in {Status} status");

			var lineItem = _lineItems.FirstOrDefault(li => li.Id == lineItemId)
				?? throw new ArgumentException($"Line item with ID {lineItemId} not found");

			lineItem.Update(
				description ?? lineItem.Description,
				unitPrice.HasValue ? new Money(unitPrice.Value, Currency) : lineItem.UnitPrice,
				quantity ?? lineItem.Quantity
			);
			RecalculateTotals();
		}

		public void ApplyDiscount(Money discountAmount)
		{
			if (Status != InvoiceStatus.Draft)
				throw new InvalidOperationException($"Cannot apply discount to invoice in {Status} status");

			if (discountAmount.Amount < 0)
				throw new ArgumentException("Discount amount cannot be negative");

			if (discountAmount.Amount > SubTotal.Amount)
				throw new ArgumentException("Discount amount cannot exceed subtotal");

			DiscountAmount = discountAmount;
			RecalculateTotals();
		}

		public void Send()
		{
			if (Status != InvoiceStatus.Draft)
				throw new InvalidOperationException($"Cannot send invoice in {Status} status");

			if (!_lineItems.Any())
				throw new InvalidOperationException("Cannot send invoice without line items");

			Status = InvoiceStatus.Sent;

			AddIntegrationEvent(new InvoiceGeneratedIntegrationEvent(
				Id, InvoiceNumber, IssuedTo, IssuedBy, TotalAmount, DueDate, BookingId, OrderId));
		}

		public void MarkAsPaid(Guid paymentId, Money amountPaid)
		{
			if (Status != InvoiceStatus.Sent && Status != InvoiceStatus.PartiallyPaid)
				throw new InvalidOperationException($"Cannot mark invoice as paid in {Status} status");

			var totalPaid = GetTotalPaidAmount() + amountPaid.Amount;

			if (totalPaid > TotalAmount.Amount)
				throw new InvalidOperationException("Total paid amount cannot exceed invoice total");

			PaidDate = DateTime.UtcNow;

			if (totalPaid == TotalAmount.Amount)
			{
				Status = InvoiceStatus.Paid;
				AddIntegrationEvent(new InvoicePaidIntegrationEvent(
					Id, InvoiceNumber, IssuedTo, paymentId, TotalAmount));
			}
			else
			{
				Status = InvoiceStatus.PartiallyPaid;
				AddIntegrationEvent(new InvoicePartiallyPaidIntegrationEvent(
					Id, InvoiceNumber, IssuedTo, paymentId, amountPaid, new Money(totalPaid, Currency)));
			}
		}

		public void MarkAsOverdue()
		{
			if (Status != InvoiceStatus.Sent && Status != InvoiceStatus.PartiallyPaid)
				throw new InvalidOperationException($"Cannot mark invoice as overdue in {Status} status");

			if (DateTime.UtcNow <= DueDate)
				throw new InvalidOperationException("Cannot mark invoice as overdue before due date");

			Status = InvoiceStatus.Overdue;

			AddIntegrationEvent(new InvoiceOverdueIntegrationEvent(
				Id, InvoiceNumber, IssuedTo, TotalAmount, DueDate));
		}

		public void Cancel(string reason)
		{
			if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Cancelled)
				throw new InvalidOperationException($"Cannot cancel invoice in {Status} status");

			Status = InvoiceStatus.Cancelled;

			AddIntegrationEvent(new InvoiceCancelledIntegrationEvent(
				Id, InvoiceNumber, IssuedTo, reason));
		}

		public void UpdateDueDate(DateTime newDueDate)
		{
			if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Cancelled)
				throw new InvalidOperationException($"Cannot update due date for invoice in {Status} status");

			if (newDueDate < DateTime.UtcNow.Date)
				throw new ArgumentException("Due date cannot be in the past");

			DueDate = newDueDate;
		}

		private void RecalculateTotals()
		{
			var subTotal = _lineItems.Sum(li => li.TotalPrice.Amount);
			SubTotal = new Money(subTotal, Currency);

			var taxAmount = (subTotal - DiscountAmount.Amount) * (decimal)(TaxRate / 100);
			TaxAmount = new Money(taxAmount, Currency);

			var totalAmount = subTotal - DiscountAmount.Amount + taxAmount;
			TotalAmount = new Money(totalAmount, Currency);
		}

		private decimal GetTotalPaidAmount()
		{
			return _payments.Where(p => p.Status == PaymentStatus.Completed)
						  .Sum(p => p.Amount.Amount);
		}

		private static void ValidateInvoiceCreation(
			Guid issuedTo,
			Guid issuedBy,
			InvoiceRecipient recipient,
			Address billingAddress,
			string currency)
		{
			if (issuedTo == Guid.Empty)
				throw new ArgumentException("IssuedTo cannot be empty");

			if (issuedBy == Guid.Empty)
				throw new ArgumentException("IssuedBy cannot be empty");

			if (issuedTo == issuedBy)
				throw new ArgumentException("IssuedTo and IssuedBy cannot be the same");

			ArgumentNullException.ThrowIfNull(recipient);
			ArgumentNullException.ThrowIfNull(billingAddress);

			if (string.IsNullOrWhiteSpace(currency))
				throw new ArgumentException("Currency cannot be empty");
		}

		private static string GenerateInvoiceNumber()
		{
			return $"INV-{DateTime.UtcNow:yyyy}-{DateTime.UtcNow.Ticks.ToString()[^6..]}";
		}

		public bool IsOverdue() => Status == InvoiceStatus.Overdue ||
								  (Status == InvoiceStatus.Sent && DateTime.UtcNow > DueDate);

		public Money GetOutstandingAmount()
		{
			var totalPaid = GetTotalPaidAmount();
			return new Money(Math.Max(0, TotalAmount.Amount - totalPaid), Currency);
		}

		public int GetDaysUntilDue() => (int)(DueDate - DateTime.UtcNow).TotalDays;
	}
}
