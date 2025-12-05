using Core.SharedKernel.DTOs;
using FluentValidation;

namespace PaymentManagement.Application.Validators.InvoiceValidators
{
	public class InvoiceUpdateDTOValidator : AbstractValidator<InvoiceUpdateDTO>
	{
		public InvoiceUpdateDTOValidator()
		{
			RuleFor(i => i.InvoiceId)
				.NotEmpty().WithMessage("Invoice ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Invoice ID cannot be empty GUID.");

			When(i => i.DueDate != null, () =>
			{
				RuleFor(i => i.DueDate)
					.GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Due date cannot be in the past.");
			});

			When(i => i.TaxRate.HasValue, () =>
			{
				RuleFor(i => i.TaxRate.Value)
					.GreaterThanOrEqualTo(0).WithMessage("Tax rate cannot be negative.")
					.LessThanOrEqualTo(100).WithMessage("Tax rate cannot exceed 100%.");
			});

			When(i => !string.IsNullOrEmpty(i.Notes), () =>
			{
				RuleFor(i => i.Notes)
					.MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
			});

			When(i => !string.IsNullOrEmpty(i.Terms), () =>
			{
				RuleFor(i => i.Terms)
					.MaximumLength(2000).WithMessage("Terms cannot exceed 2000 characters.");
			});

			When(i => !string.IsNullOrEmpty(i.BillingStreet), () =>
			{
				RuleFor(i => i.BillingStreet)
					.MaximumLength(200).WithMessage("Billing street cannot exceed 200 characters.");
			});

			When(i => !string.IsNullOrEmpty(i.BillingCity), () =>
			{
				RuleFor(i => i.BillingCity)
					.MaximumLength(100).WithMessage("Billing city cannot exceed 100 characters.");
			});

			When(i => !string.IsNullOrEmpty(i.BillingPostalCode), () =>
			{
				RuleFor(i => i.BillingPostalCode)
					.MaximumLength(20).WithMessage("Billing postal code cannot exceed 20 characters.");
			});

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

			When(i => !string.IsNullOrEmpty(i.Status), () =>
			{
				RuleFor(i => i.Status)
					.IsInEnum().WithMessage("Invoice status must be a valid enum value.");
			});
		}
	}
}
