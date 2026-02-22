using FluentValidation;
using NotificationManagement.Application.CQRS.Commands.NotificationCommands;

namespace NotificationManagement.Application.Validators
{
	/// <summary>
	/// Validator for SendBookingConfirmationNotificationCommand
	/// Ensures all required fields for booking confirmation notifications are valid
	/// </summary>
	public class SendBookingConfirmationNotificationCommandValidator : AbstractValidator<SendBookingConfirmationNotificationCommand>
	{
		public SendBookingConfirmationNotificationCommandValidator()
		{
			// Correlation ID validation
			RuleFor(x => x.CorrelationId)
				.NotEmpty().WithMessage("CorrelationId is required")
				.Must(id => id != Guid.Empty).WithMessage("CorrelationId must be a valid GUID");

			// Booking ID validation
			RuleFor(x => x.BookingId)
				.NotEmpty().WithMessage("BookingId is required")
				.Must(id => id != Guid.Empty).WithMessage("BookingId must be a valid GUID");

			// Recipient ID validation
			RuleFor(x => x.RecipientId)
				.NotEmpty().WithMessage("RecipientId is required")
				.Must(id => id != Guid.Empty).WithMessage("RecipientId must be a valid GUID");

			// Customer Email validation
			RuleFor(x => x.CustomerEmail)
				.NotEmpty().WithMessage("CustomerEmail is required")
				.EmailAddress().WithMessage("CustomerEmail must be a valid email address")
				.MaximumLength(255).WithMessage("CustomerEmail cannot exceed 255 characters");

			// Service Description validation
			RuleFor(x => x.ServiceDescription)
				.NotEmpty().WithMessage("ServiceDescription is required")
				.MaximumLength(500).WithMessage("ServiceDescription cannot exceed 500 characters")
				.MinimumLength(3).WithMessage("ServiceDescription must be at least 3 characters");

			// Amount validation
			RuleFor(x => x.Amount)
				.NotEmpty().WithMessage("Amount is required")
				.GreaterThan(0).WithMessage("Amount must be greater than 0")
				.PrecisionScale(18, 2, true).WithMessage("Amount must have a maximum of 2 decimal places");

			// Currency validation
			RuleFor(x => x.Currency)
				.NotEmpty().WithMessage("Currency is required")
				.Length(3).WithMessage("Currency must be a 3-letter ISO 4217 code")
				.Matches("^[A-Z]{3}$").WithMessage("Currency must be a valid ISO 4217 currency code (e.g., USD, EUR, GBP)");

			// Scheduled Date validation (optional, but if provided must be valid)
			RuleFor(x => x.ScheduledDate)
				.Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
				.WithMessage("ScheduledDate must be in the future if provided");

			// Payment Reference validation (optional, but if provided must have content)
			RuleFor(x => x.PaymentReference)
				.MaximumLength(100).WithMessage("PaymentReference cannot exceed 100 characters")
				.When(x => !string.IsNullOrWhiteSpace(x.PaymentReference));
		}
	}
}
