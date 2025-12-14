using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace UserManagement.Domain.Entities
{
	public class Customer : User
	{
		public Address? Address { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PaymentMethod PreferredPaymentMethod { get; set; }
		private List<ServiceRequest> _serviceRequests = [];
		private List<InboxMessage> _messages = [];
		private List<Proposal> _proposals = [];
		public IReadOnlyCollection<ServiceRequest> ServiceRequests => _serviceRequests.AsReadOnly();
		public IReadOnlyCollection<InboxMessage> Messages => _messages.AsReadOnly();
		public IReadOnlyCollection<Proposal> Proposals => _proposals.AsReadOnly();

		private Customer() : base() { }
		public Customer(Email email, Address address, PaymentMethod preferredPaymentMethod) 
			: base(email, UserRole.Customer)
		{
			Address = address;
			PreferredPaymentMethod = preferredPaymentMethod;
		}
	}
}
