using Core.SharedKernel.Enums;

namespace Core.SharedKernel.ValueObjects
{
	public record InvoiceRecipient
	{
		public string Name { get; }
		public string? CompanyName { get; }
		public Email Email { get; }
		public PhoneNumber? PhoneNumber { get; }
		public string? TaxId { get; }
		public string? RegistrationNumber { get; }
		public InvoiceRecipientType Type { get; }

		private InvoiceRecipient() { }

		public InvoiceRecipient(
			string name,
			Email email,
			InvoiceRecipientType type,
			string? companyName = null,
			PhoneNumber? phoneNumber = null,
			string? taxId = null,
			string? registrationNumber = null)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Recipient name cannot be empty", nameof(name));

			Name = name.Trim();
			Email = email ?? throw new ArgumentNullException(nameof(email));
			Type = type;
			CompanyName = string.IsNullOrWhiteSpace(companyName) ? null : companyName.Trim();
			PhoneNumber = phoneNumber;
			TaxId = string.IsNullOrWhiteSpace(taxId) ? null : taxId.Trim();
			RegistrationNumber = string.IsNullOrWhiteSpace(registrationNumber) ? null : registrationNumber.Trim();

			ValidateBusinessRules();
		}

		public static InvoiceRecipient CreateForIndividual(
			string fullName,
			Email email,
			PhoneNumber? phoneNumber = null)
		{
			return new InvoiceRecipient(
				fullName,
				email,
				InvoiceRecipientType.Individual,
				phoneNumber: phoneNumber);
		}

		public static InvoiceRecipient CreateForBusiness(
			string contactName,
			string companyName,
			Email email,
			PhoneNumber? phoneNumber = null,
			string? taxId = null,
			string? registrationNumber = null)
		{
			return new InvoiceRecipient(
				contactName,
				email,
				InvoiceRecipientType.Business,
				companyName,
				phoneNumber,
				taxId,
				registrationNumber);
		}

		public static InvoiceRecipient CreateFromUser(Guid userId, string name, Email email, InvoiceRecipientType type)
		{
			// This would typically fetch additional user details from UserManagement module
			return new InvoiceRecipient(name, email, type);
		}

		private void ValidateBusinessRules()
		{
			switch (Type)
			{
				case InvoiceRecipientType.Business:
					if (string.IsNullOrWhiteSpace(CompanyName))
						throw new ArgumentException("Company name is required for business recipients");
					break;

				case InvoiceRecipientType.Individual:
					break;

				default:
					throw new ArgumentException($"Unknown recipient type: {Type}");
			}
		}

		public string GetDisplayName()
		{
			return Type switch
			{
				InvoiceRecipientType.Business => $"{Name} ({CompanyName})",
				InvoiceRecipientType.Individual => Name,
				_ => Name
			};
		}

		public string GetFullContactInfo()
		{
			var info = GetDisplayName();

			if (PhoneNumber != null)
				info += $" - {PhoneNumber.CountryCode} {PhoneNumber.Number}";

			info += $" - {Email.Address}";

			if (!string.IsNullOrWhiteSpace(TaxId))
				info += $" (Tax ID: {TaxId})";

			return info;
		}

		public bool IsBusinessRecipient() => Type == InvoiceRecipientType.Business;
		public bool IsIndividualRecipient() => Type == InvoiceRecipientType.Individual;
	}
}
