using Core.SharedKernel.DTOs;
using FluentValidation;

namespace PaymentManagement.Application.Validators.PaymentValidators
{
	public class PaymentUpdateDTOValidator : AbstractValidator<PaymentUpdateDTO>
	{
		public PaymentUpdateDTOValidator()
		{
			RuleFor(p => p.PaymentId)
				.NotEmpty().WithMessage("Payment ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Payment ID cannot be empty GUID.");

			RuleFor(p => p.Description)
				.MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

			When(p => !string.IsNullOrEmpty(p.BillingStreet), () =>
			{
				RuleFor(p => p.BillingStreet)
					.MaximumLength(200).WithMessage("Billing street cannot exceed 200 characters.");
			});

			When(p => !string.IsNullOrEmpty(p.BillingCity), () =>
			{
				RuleFor(p => p.BillingCity)
					.MaximumLength(100).WithMessage("Billing city cannot exceed 100 characters.");
			});

			When(p => !string.IsNullOrEmpty(p.BillingPostalCode), () =>
			{
				RuleFor(p => p.BillingPostalCode)
					.MaximumLength(20).WithMessage("Billing postal code cannot exceed 20 characters.")
					.Matches(@"^[A-Za-z0-9\s-]+$").WithMessage("Postal code contains invalid characters.");
			});

			//When(p => !string.IsNullOrEmpty(p.BillingState), () =>
			//{
			//	RuleFor(p => p.BillingState)
			//		.MaximumLength(100).WithMessage("Billing state cannot exceed 100 characters.");
			//});

			//When(p => !string.IsNullOrEmpty(p.BillingCountry), () =>
			//{
			//	RuleFor(p => p.BillingCountry)
			//		.MaximumLength(100).WithMessage("Billing country cannot exceed 100 characters.");
			//});

			When(p => !string.IsNullOrEmpty(p.Status), () =>
			{
				RuleFor(p => p.Status)
					.IsInEnum().WithMessage("Payment status must be a valid enum value.");
			});

			RuleFor(p => p.ExternalTransactionId)
				.MaximumLength(200).WithMessage("External transaction ID cannot exceed 200 characters.");

			RuleFor(p => p.PaymentIntentId)
				.MaximumLength(200).WithMessage("Payment intent ID cannot exceed 200 characters.");

			RuleFor(p => p.FailureReason)
				.MaximumLength(500).WithMessage("Failure reason cannot exceed 500 characters.");
		}
	}
}
