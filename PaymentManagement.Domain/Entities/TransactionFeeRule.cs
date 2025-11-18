using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PaymentManagement.Domain.Entities
{
	public class TransactionFeeRule : AggregateRoot
	{
		public string RuleName { get; private set; } = string.Empty;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public FeeType FeeType { get; private set; }
		[Column(TypeName = "decimal(18, 2)")]
		public decimal FeeValue { get; private set; }
		public string AppliesTo { get; private set; } = string.Empty;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public TransactionFeeRuleStatus TransactionFeeRuleStatus { get; private set; }

		private TransactionFeeRule() { }

		public void Deactivate()
		{
			TransactionFeeRuleStatus = TransactionFeeRuleStatus.Inactive;
		}

		public void Update(string newType, decimal newValue, string appliesTo)
		{
			FeeType = Enum.Parse<FeeType>(newType, true);
			FeeValue = newValue;
			AppliesTo = appliesTo;
		}

		public Money CalculateFee(Money originalAmount)
		{
			if(FeeType == FeeType.Percentage)
			{
				return new Money(originalAmount.Amount * (FeeValue / 100), "");
			}
			return new Money(FeeValue, "");
		}
	}
}
