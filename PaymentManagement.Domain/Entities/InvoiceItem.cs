using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace PaymentManagement.Domain.Entities
{
	public class InvoiceItem : Entity
	{
		public Guid InvoiceId { get; private set; }
		public NewInvoice Invoice { get; private set; }
		public string Description { get; private set; } = string.Empty;
		public Money Amount { get; set; } = new(0, "");

		private InvoiceItem() { }

		public static InvoiceItem Create(decimal amount, string code = "")
		{
			return new InvoiceItem
			{
				Amount = new(amount, code)
			};
		}
	}
}
