using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Core.SharedKernel.IntegrationEvents.RefundIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentManagement.Domain.Entities
{
	public class Payment : AggregateRoot
	{
		public Money Amount { get; private set; }
		public string Currency { get; private set; }
		public string Reference { get; private set; }
		public string? ExternalTransactionId { get; private set; }
		public string? PaymentIntentId { get; private set; }
		public string? Description { get; private set; }
		public Guid? InvoiceId { get; private set; }
		public Guid? BookingId { get; private set; }
		public Guid? OrderId { get; private set; }
		public Guid? PayerId { get; private set; }
		public Guid? RecipientId { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PaymentMethod PaymentMethod { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PaymentStatus Status { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PaymentType PaymentType { get; private set; }
		public Address BillingAddress { get; private set; }
		public string? FailureReason { get; private set; }
		public DateTime CreatedAt { get; private set; }
		public DateTime ModifiedAt { get; private set; }
		public DateTime? ProcessedAt { get; private set; }
		public DateTime? AuthorizedAt { get; private set; }
		public DateTime? CapturedAt { get; private set; }
		[Timestamp]
		public byte[] RowVersion { get; set; }
		private readonly List<PaymentTransaction> _transactions = [];
		public IReadOnlyCollection<PaymentTransaction> Transactions => _transactions.AsReadOnly();
		private readonly List<Refund> _refunds = [];
		public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();
		public bool IsDeleted { get; private set; }
		public DateTime? DeletedAt { get; private set; }
		public string? DeletedBy { get; private set; }

		private Payment() { }

		public static Payment CreateForBooking(
			Guid correlationId, Guid bookingId, Money amount, Guid payerId,
			Guid recipientId, PaymentMethod paymentMethod, Address billingAddress,
			string reference, string paymentType = "Booking", string? description = null)
		{
			ValidatePaymentCreation(amount, payerId, recipientId, paymentMethod, billingAddress);
			var payment = new Payment
			{
				Id = Guid.NewGuid(),
				BookingId = bookingId,
				Amount = amount,
				PaymentType = Enum.Parse<PaymentType>(paymentType, true),
				PayerId = payerId,
				RecipientId = recipientId,
				PaymentMethod = paymentMethod,
				BillingAddress = billingAddress,
				Description = description,
				Reference = reference,
				Status = PaymentStatus.Pending,
				CreatedAt = DateTime.UtcNow
			};
			payment.AddIntegrationEvent(new PaymentInitiatedIntegrationEvent(
				correlationId, payment.Id, bookingId, amount, payerId, recipientId));
			return payment;
		}

		public static Payment CreateForOrder(
			Guid correlationId, Guid orderId, Money amount, Guid payerId,
			Guid recipientId, PaymentMethod paymentMethod, Address billingAddress,
			string reference, string paymentType = "Order", string? description = null)
		{
			ValidatePaymentCreation(amount, payerId, recipientId, paymentMethod, billingAddress);
			var payment = new Payment
			{
				Id = Guid.NewGuid(),
				OrderId = orderId,
				Amount = amount,
				PayerId = payerId,
				RecipientId = recipientId,
				PaymentMethod = paymentMethod,
				PaymentType = Enum.Parse<PaymentType>(paymentType, true),
				BillingAddress = billingAddress,
				Reference = reference,
				Description = description,
				Status = PaymentStatus.Pending,
				CreatedAt = DateTime.UtcNow
			};
			payment.AddIntegrationEvent(new PaymentInitiatedIntegrationEvent(
				correlationId, payment.Id, orderId, amount, payerId, recipientId));
			return payment;
		}

		public static Payment CreateForInvoice(
			Guid correlationId, Guid invoiceId, Money amount, Guid payerId,
			Guid recipientId, PaymentMethod paymentMethod, Address billingAddress,
			string reference, string paymentType = "Invoice", string? description = null)
		{
			ValidatePaymentCreation(amount, payerId, recipientId, paymentMethod, billingAddress);
			var payment = new Payment
			{
				Id = Guid.NewGuid(),
				InvoiceId = invoiceId,
				Amount = amount,
				PayerId = payerId,
				RecipientId = recipientId,
				PaymentMethod = paymentMethod,
				PaymentType = Enum.Parse<PaymentType>(paymentType, true),
				BillingAddress = billingAddress,
				Reference = reference,
				Description = description,
				Status = PaymentStatus.Pending,
				CreatedAt = DateTime.UtcNow
			};
			payment.AddIntegrationEvent(new PaymentInitiatedIntegrationEvent(
				correlationId, payment.Id, invoiceId, amount, payerId, recipientId));
			return payment;
		}

		public void Authorize(Guid correlationId, string externalTransactionId, string? paymentIntentId)
		{
			if (Status != PaymentStatus.Pending)
				throw new InvalidOperationException("Only pending payments can be authorized.");
			ExternalTransactionId = externalTransactionId;
			PaymentIntentId = paymentIntentId;
			Status = PaymentStatus.Authorized;
			AuthorizedAt = DateTime.UtcNow;
			AddIntegrationEvent(new PaymentAuthorizedIntegrationEvent(
				correlationId, Id, Amount, PayerId, RecipientId));
			AddTransaction(PaymentTransactionType.Authorization, Amount, "Payment authorized");
		}

		public void Capture(Guid correlationId)
		{
			if (Status != PaymentStatus.Authorized)
				throw new InvalidOperationException("Only authorized payments can be captured.");
			Status = PaymentStatus.Completed;
			CapturedAt = DateTime.UtcNow;
			ProcessedAt = DateTime.UtcNow;
			AddIntegrationEvent(new PaymentCompletedIntegrationEvent(
				correlationId, Id, BookingId, OrderId, InvoiceId, Amount, PayerId, RecipientId));
			AddTransaction(PaymentTransactionType.Capture, Amount, "Payment captured");
		}

		public void Complete(Guid correlationId, string externalTransactionId)
		{
			if(Status != PaymentStatus.Pending && Status != PaymentStatus.Authorized)
				throw new InvalidOperationException("Only pending or authorized payments can be completed.");
			ExternalTransactionId = externalTransactionId;
			Status = PaymentStatus.Completed;
			ProcessedAt = DateTime.UtcNow;
			CapturedAt = DateTime.UtcNow;

			AddTransaction(PaymentTransactionType.Payment, Amount, "Payment completed");

			AddIntegrationEvent(new PaymentCompletedIntegrationEvent(
				correlationId, Id, BookingId, OrderId, InvoiceId, Amount, PayerId, RecipientId));
		}

		public void Fail(Guid correlationId, string reason)
		{
			if (Status == PaymentStatus.Completed || Status == PaymentStatus.Refunded)
				throw new InvalidOperationException($"Cannot fail payment in {Status} status.");
			Status = PaymentStatus.Failed;
			FailureReason = reason;
			ProcessedAt = DateTime.UtcNow;

			AddIntegrationEvent(new PaymentFailedIntegrationEvent(
				correlationId, Id, BookingId, OrderId, InvoiceId, reason, PayerId, RecipientId));
			AddTransaction(PaymentTransactionType.Failure, new Money(0, Currency), reason);
		}

		public Refund ProcessRefund(Money refundAmount, string reason, Guid initiatedBy)
		{
			if (Status != PaymentStatus.Completed)
				throw new InvalidOperationException($"Cannot refund payment in {Status} status");

			if (refundAmount.Amount <= 0)
				throw new ArgumentException("Refund amount must be greater than zero");

			var totalRefunded = _refunds.Where(r => r.Status == RefundStatus.Processed)
									  .Sum(r => r.Amount.Amount);

			if (totalRefunded + refundAmount.Amount > Amount.Amount)
				throw new InvalidOperationException("Total refund amount cannot exceed payment amount");

			var refund = Refund.Create(Id, refundAmount, reason, initiatedBy);
			_refunds.Add(refund);

			AddTransaction(PaymentTransactionType.Refund, refundAmount, reason);

			if (totalRefunded + refundAmount.Amount == Amount.Amount)
			{
				Status = PaymentStatus.Refunded;
			}
			else
			{
				Status = PaymentStatus.PartiallyRefunded;
			}

			AddIntegrationEvent(new RefundProcessedIntegrationEvent(
				Guid.NewGuid(), refund.Id, Id, refundAmount, reason, initiatedBy));

			return refund;
		}

		public void Cancel(Guid correlationId, string reason)
		{
			if (Status != PaymentStatus.Pending || Status != PaymentStatus.Authorized)
				throw new InvalidOperationException($"Cannot cancel payment in {Status} status");

			Status = PaymentStatus.Cancelled;
			FailureReason = reason;
			ProcessedAt = DateTime.UtcNow;
			ModifiedAt = DateTime.UtcNow;

			AddTransaction(PaymentTransactionType.Cancellation, new Money(0, Currency), reason);

			AddIntegrationEvent(new PaymentCancelledIntegrationEvent(
				correlationId, Id, BookingId, OrderId, InvoiceId, reason, PayerId, RecipientId));
		}

		public void SoftDelete(Guid correlationId, string deletedBy, string reason)
		{
			if (Status == PaymentStatus.Completed || Status == PaymentStatus.Refunded)
				throw new InvalidOperationException("Cannot delete completed or refunded payments");
			Cancel(correlationId, reason);
			IsDeleted = true;
			DeletedAt = DateTime.UtcNow;
			DeletedBy = deletedBy;
		}

		private void AddTransaction(PaymentTransactionType type, Money amount, string description)
		{
			var transaction = PaymentTransaction.Create(Id, type, amount, description);
			_transactions.Add(transaction);
		}

		private static void ValidatePaymentCreation(
			Money amount,
			Guid payerId,
			Guid recipientId,
			PaymentMethod paymentMethod,
			Address billingAddress)
		{
			if (amount.Amount <= 0)
				throw new ArgumentException("Payment amount must be greater than zero");

			if (payerId == Guid.Empty)
				throw new ArgumentException("Payer ID cannot be empty");

			if (recipientId == Guid.Empty)
				throw new ArgumentException("Recipient ID cannot be empty");

			if (payerId == recipientId)
				throw new ArgumentException("Payer and recipient cannot be the same");

			ArgumentNullException.ThrowIfNull(paymentMethod);
			ArgumentNullException.ThrowIfNull(billingAddress);
		}

		public bool CanBeRefunded() => Status == PaymentStatus.Completed;

		public Money GetAvailableRefundAmount()
		{
			if (!CanBeRefunded()) return new Money(0, Currency);

			var totalRefunded = _refunds.Where(r => r.Status == RefundStatus.Processed)
									  .Sum(r => r.Amount.Amount);

			return new Money(Amount.Amount - totalRefunded, Currency);
		}
	}
}
