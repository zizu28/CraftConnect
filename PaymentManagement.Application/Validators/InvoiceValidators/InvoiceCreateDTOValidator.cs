using Core.SharedKernel.Enums;
using FluentValidation;
using FluentValidation.Validators;
using PaymentManagement.Application.DTOs.InvoiceDTOS;
using PaymentManagement.Application.Validators.InvoiceLineItemValidators;

namespace PaymentManagement.Application.Validators.InvoiceValidators
{
	public class InvoiceCreateDTOValidator : AbstractValidator<InvoiceCreateDTO>
	{
		public InvoiceCreateDTOValidator()
		{
			RuleFor(i => i.InvoiceType)
				.NotEmpty().WithMessage("Invoice type must be a valid enum value.");

			RuleFor(i => i.IssuedTo)
				.NotEmpty().WithMessage("Issued to ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Issued to ID cannot be empty GUID.");

			RuleFor(i => i.IssuedBy)
				.NotEmpty().WithMessage("Issued by ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Issued by ID cannot be empty GUID.");

			RuleFor(i => i)
				.Must(i => i.IssuedTo != i.IssuedBy)
				.WithMessage("Issued to and issued by cannot be the same.")
				.WithName("IssuedTo/IssuedBy");

			When(i => Enum.Parse<InvoiceType>(i.InvoiceType) == InvoiceType.Booking, () =>
			{
				RuleFor(i => i.BookingId)
					.NotEmpty().WithMessage("Booking ID is required for booking invoices.")
					.Must(id => id != Guid.Empty).WithMessage("Booking ID cannot be empty GUID.");
			});

			When(i => Enum.Parse<InvoiceType>(i.InvoiceType) == InvoiceType.Order, () =>
			{
				RuleFor(i => i.OrderId)
					.NotEmpty().WithMessage("Order ID is required for order invoices.")
					.Must(id => id != Guid.Empty).WithMessage("Order ID cannot be empty GUID.");
			});

			When(i => i.DueDate != null, () =>
			{
				RuleFor(i => i.DueDate)
					.GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Due date cannot be in the past.");
			});

			RuleFor(i => i.Currency)
				.NotEmpty().WithMessage("Currency is required.")
				.Length(3).WithMessage("Currency must be exactly 3 characters.")
				.Matches(@"^[A-Z]{3}$").WithMessage("Currency must be a valid 3-letter ISO currency code.");

			RuleFor(i => i.TaxRate)
				.GreaterThanOrEqualTo(0).WithMessage("Tax rate cannot be negative.")
				.LessThanOrEqualTo(100).WithMessage("Tax rate cannot exceed 100%.");

			RuleFor(i => i.Notes)
				.MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");

			RuleFor(i => i.Terms)
				.MaximumLength(2000).WithMessage("Terms cannot exceed 2000 characters.");

			// Invoice Recipient validation
			RuleFor(i => i.RecipientName)
				.NotEmpty().WithMessage("Recipient name is required.")
				.MaximumLength(200).WithMessage("Recipient name cannot exceed 200 characters.");

			When(i => Enum.Parse<InvoiceRecipientType>(i.RecipientType) == InvoiceRecipientType.Business, () =>
			{
				RuleFor(i => i.RecipientCompanyName)
					.NotEmpty().WithMessage("Company name is required for business recipients.")
					.MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");
			});

			RuleFor(i => i.RecipientEmail)
				.NotEmpty().WithMessage("Recipient email is required.")
				.EmailAddress().WithMessage("Recipient email must be a valid email address.");

			When(i => !string.IsNullOrEmpty(i.RecipientPhone), () =>
			{
				RuleFor(i => i.RecipientPhone)
					.Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Recipient phone must be a valid phone number.");
			});

			RuleFor(i => i.RecipientType)
				.IsInEnum().WithMessage("Recipient type must be a valid enum value.");

			When(i => !string.IsNullOrEmpty(i.RecipientTaxId), () =>
			{
				RuleFor(i => i.RecipientTaxId)
					.MaximumLength(50).WithMessage("Tax ID cannot exceed 50 characters.");
			});

			When(i => !string.IsNullOrEmpty(i.RecipientRegistrationNumber), () =>
			{
				RuleFor(i => i.RecipientRegistrationNumber)
					.MaximumLength(100).WithMessage("Registration number cannot exceed 100 characters.");
			});

			RuleFor(i => i.BillingStreet)
				.NotEmpty().WithMessage("Billing street is required.")
				.MaximumLength(200).WithMessage("Billing street cannot exceed 200 characters.");

			RuleFor(i => i.BillingCity)
				.NotEmpty().WithMessage("Billing city is required.")
				.MaximumLength(100).WithMessage("Billing city cannot exceed 100 characters.");

			RuleFor(i => i.BillingPostalCode)
				.NotEmpty().WithMessage("Billing postal code is required.")
				.MaximumLength(20).WithMessage("Billing postal code cannot exceed 20 characters.");

			//When(i => !string.IsNullOrEmpty(i.BillingState), () =>
			//{
			//	RuleFor(i => i.BillingState)
			//		.MaximumLength(100).WithMessage("Billing state cannot exceed 100 characters.");
			//});

			//When(i => !string.IsNullOrEmpty(i.BillingCountry), () =>
			//{
			//	RuleFor(i => i.BillingCountry)
			//		.MaximumLength(100).WithMessage("Billing country cannot exceed 100 characters.");
			//});

			RuleFor(i => i.LineItems)
				.NotEmpty().WithMessage("At least one line item is required.")
				.Must(items => items.Count <= 100).WithMessage("Cannot have more than 100 line items per invoice.");

			RuleForEach(i => i.LineItems).SetValidator(new InvoiceLineItemCreateDTOValidator());
		}
	}
}
