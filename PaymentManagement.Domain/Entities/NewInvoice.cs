using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace PaymentManagement.Domain.Entities
{
	public class NewInvoice : AggregateRoot
	{
		public Guid ProjectId { get; private set; }
		public Guid CustomerId { get; private set; }
		public Guid CraftsmanId { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public InvoiceStatus InvoiceStatus { get; private set; }
		public List<InvoiceItem> Items { get; private set; } = [];
		public Money TotalAmount { get; private set; } = new(0, "");

		private NewInvoice() { }

		public static NewInvoice CreateFromProposal(Proposal proposal, TransactionFeeRule transactionFeeRule)
		{
			var invoice = new NewInvoice
			{
				InvoiceStatus = InvoiceStatus.Pending
			};

			var invoiceItem = InvoiceItem.Create(proposal.Price);
			invoice.Items.Add(invoiceItem);
			var transactionFee = transactionFeeRule.CalculateFee(new Money(proposal.Price, ""));
			invoice.Items.Add(InvoiceItem.Create(transactionFee.Amount));
			invoice.TotalAmount = new(invoice.Items.Sum(item => item.Amount.Amount), "");
			return invoice;
		}

		public void MarkAsPaid()
		{
			if(InvoiceStatus == InvoiceStatus.Pending)
			{
				InvoiceStatus = InvoiceStatus.Paid;
			}
			AddIntegrationEvent(new InvoicePaid(Id, ProjectId, CraftsmanId, CustomerId, TotalAmount.Amount));
		}

		public void IssueRefund(Money amount, string reason)
		{
			InvoiceStatus = InvoiceStatus.Refunded;
			AddIntegrationEvent(new RefundIssued(Id, ProjectId, amount.Amount, amount.Currency, reason));
		}
	}
}
