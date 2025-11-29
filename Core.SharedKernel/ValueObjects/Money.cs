using System.ComponentModel.DataAnnotations.Schema;

namespace Core.SharedKernel.ValueObjects
{
	public record Money
	{
		[Column(TypeName = "decimal(18, 2)")]
		public decimal Amount { get; private set; }
		public string Currency { get; private set; }

		private Money()
		{
			Amount = 0;
			Currency = string.Empty;
		}
		public Money(decimal amount, string currency)
		{
			if (amount < 0)
				throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
			Amount = amount;
			Currency = currency ?? throw new ArgumentNullException(nameof(currency), "Currency cannot be null.");
		}
		public static Money operator +(Money a, Money b)
		{
			if (a.Currency != b.Currency)
				throw new InvalidOperationException("Cannot add Money with different currencies.");
			return new Money(a.Amount + b.Amount, a.Currency);
		}
		public static Money operator -(Money a, Money b)
		{
			if (a.Currency != b.Currency)
				throw new InvalidOperationException("Cannot subtract Money with different currencies.");
			return new Money(a.Amount - b.Amount, a.Currency);
		}

		public static Money operator *(Money a, decimal b)
		{
			return new Money(a.Amount * b, a.Currency);
		}

		public static Money operator /(Money a, decimal b)
		{
			if (b == 0)
				throw new DivideByZeroException("Cannot divide by zero.");
			return new Money(a.Amount / b, a.Currency);
		}
	}
}
