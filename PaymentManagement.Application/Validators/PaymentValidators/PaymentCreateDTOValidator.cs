using FluentValidation;
using PaymentManagement.Application.DTOs.PaymentDTOs;

namespace PaymentManagement.Application.Validators.PaymentValidators
{
	public class PaymentCreateDTOValidator : AbstractValidator<PaymentCreateDTO>
	{
		public PaymentCreateDTOValidator()
		{
			RuleFor(p => p.Amount)
				.GreaterThan(0).WithMessage("Payment amount must be greater than zero.")
				.LessThanOrEqualTo(1000000).WithMessage("Payment amount cannot exceed 1,000,000.");

			RuleFor(p => p.Currency)
				.NotEmpty().WithMessage("Currency is required.")
				.Length(3).WithMessage("Currency must be exactly 3 characters.")
				.Matches(@"^[A-Z]{3}$").WithMessage("Currency must be a valid 3-letter ISO currency code (e.g., USD, EUR).");

			RuleFor(p => p.PaymentMethod)
				.IsInEnum().WithMessage("Payment method must be a valid enum value.");

			RuleFor(p => p.PaymentType)
				.IsInEnum().WithMessage("Payment type must be a valid enum value.");

			RuleFor(p => p.PaymentStatus)
				.IsInEnum().WithMessage("Payment status must be a valid enum value.");

			RuleFor(p => p.PayerId)
				.NotEmpty().WithMessage("Payer ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Payer ID cannot be empty GUID.");

			RuleFor(p => p.RecipientId)
				.NotEmpty().WithMessage("Recipient ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Recipient ID cannot be empty GUID.");

			RuleFor(p => p)
				.Must(p => p.PayerId != p.RecipientId)
				.WithMessage("Payer and recipient cannot be the same.")
				.WithName("PayerId/RecipientId");

			RuleFor(p => p)
				.Must(p => p.BookingId != null || p.OrderId != null || p.InvoiceId != null)
				.WithMessage("At least one context (BookingId, OrderId, or InvoiceId) must be provided.")
				.WithName("Context");

			RuleFor(p => p.Description)
				.MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

			RuleFor(p => p.BillingStreet)
				.NotEmpty().WithMessage("Billing street is required.")
				.MaximumLength(200).WithMessage("Billing street cannot exceed 200 characters.");

			RuleFor(p => p.BillingCity)
				.NotEmpty().WithMessage("Billing city is required.")
				.MaximumLength(100).WithMessage("Billing city cannot exceed 100 characters.");

			RuleFor(p => p.BillingPostalCode)
				.NotEmpty().WithMessage("Billing postal code is required.")
				.MaximumLength(20).WithMessage("Billing postal code cannot exceed 20 characters.")
				.Matches(@"^[A-Za-z0-9\s-]+$").WithMessage("Postal code contains invalid characters.");

			When(p => !string.IsNullOrEmpty(p.CardLast4Digits), () =>
			{
				RuleFor(p => p.CardLast4Digits)
					.Length(4).WithMessage("Card last 4 digits must be exactly 4 characters.")
					.Matches(@"^\d{4}$").WithMessage("Card last 4 digits must contain only numbers.");
			});

			When(p => !string.IsNullOrEmpty(p.CardBrand), () =>
			{
				RuleFor(p => p.CardBrand)
					.MaximumLength(50).WithMessage("Card brand cannot exceed 50 characters.");
			});

			When(p => p.CardExpiryDate.HasValue, () =>
			{
				RuleFor(p => p.CardExpiryDate!.Value)
					.GreaterThan(DateTime.UtcNow).WithMessage("Card expiry date must be in the future.");
			});

			RuleFor(p => p.ExternalMethodId)
				.MaximumLength(100).WithMessage("External method ID cannot exceed 100 characters.");
		}
	}
}
