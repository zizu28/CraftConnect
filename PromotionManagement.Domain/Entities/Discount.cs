using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace PromotionManagement.Domain.Entities
{
	public class Discount : AggregateRoot
	{
		public string Code { get; private set; } = string.Empty;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DiscountType DiscountType { get; private set; }
		public decimal Value { get; private set; }
		public DateTime ExpiryDate { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DiscountStatus DiscountStatus { get; private set; }
		private Redemption Redemption { get; set; } = new(0, 0);

		private Discount() { }

		public static Discount Create(string code, DiscountType discountType, decimal value, int count, int limit, DateTime expiryDate)
		{
			return new Discount
			{
				Code = code,
				DiscountType = discountType,
				Value = value,
				ExpiryDate = expiryDate,
				DiscountStatus = DiscountStatus.Active,
				Redemption = new Redemption(count, limit)
			};
		}

		public bool IsValid()
		{
			if(DiscountStatus == DiscountStatus.Active && Redemption.Count < Redemption.Limit &&
				ExpiryDate > DateTime.UtcNow)
			{
				return true;
			}
			return false;
		}

		public bool Redeem(int count, int limit)
		{
			if (!IsValid()) return false;
			Redemption = new Redemption(count++, limit);
			if (Redemption.Count == Redemption.Limit) DiscountStatus = DiscountStatus.Expired;
			return true;
		}

		public void Deactivate()
		{
			DiscountStatus = DiscountStatus.Inactive;
		}

		public decimal Apply(Money originalAmount)
		{
			decimal discount = 0;
			if (DiscountType == DiscountType.Percentage)
			{
				discount = originalAmount.Amount * (Value / 100);
			}
			if(DiscountType == DiscountType.FixedAmount)
			{
				discount = Value;
			}
			return originalAmount.Amount - discount;
		}
	}
}
