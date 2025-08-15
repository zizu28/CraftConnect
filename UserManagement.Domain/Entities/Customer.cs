using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace UserManagement.Domain.Entities
{
	public class Customer : User
	{
		public UserAddress? Address { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PaymentMethod PreferredPaymentMethod { get; set; }

		private Customer() : base() { }
		public Customer(Email email, UserAddress address, PaymentMethod preferredPaymentMethod) 
			: base(email, UserRole.Customer)
		{
			Address = address;
			PreferredPaymentMethod = preferredPaymentMethod;
		}
	}
}
