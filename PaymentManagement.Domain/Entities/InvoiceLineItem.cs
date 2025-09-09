using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace PaymentManagement.Domain.Entities
{
	public class InvoiceLineItem : Entity
	{
		public Guid InvoiceId { get; private set; }
		public string Description { get; private set; }
		public Money UnitPrice { get; private set; }
		public int Quantity { get; private set; }
		public Money TotalPrice { get; private set; }
		public string? ItemCode { get; private set; }

		private InvoiceLineItem() { }

		public static InvoiceLineItem Create(
			Guid invoiceId,
			string description,
			Money unitPrice,
			int quantity,
			string? itemCode = null)
		{
			var lineItem = new InvoiceLineItem
			{
				Id = Guid.NewGuid(),
				InvoiceId = invoiceId,
				Description = description,
				UnitPrice = unitPrice,
				Quantity = quantity,
				ItemCode = itemCode
			};

			lineItem.CalculateTotalPrice();
			return lineItem;
		}

		public void Update(string description, Money unitPrice, int quantity)
		{
			Description = description;
			UnitPrice = unitPrice;
			Quantity = quantity;
			CalculateTotalPrice();
		}

		private void CalculateTotalPrice()
		{
			TotalPrice = new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);
		}
	}
}
