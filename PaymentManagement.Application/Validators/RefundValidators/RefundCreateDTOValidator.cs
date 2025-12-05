using Core.SharedKernel.DTOs;
using FluentValidation;

namespace PaymentManagement.Application.Validators.RefundValidators
{
	public class RefundCreateDTOValidator : AbstractValidator<RefundCreateDTO>
	{
		public RefundCreateDTOValidator()
		{
			RuleFor(r => r.PaymentId)
				.NotEmpty().WithMessage("Payment ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Payment ID cannot be empty GUID.");

			RuleFor(r => r.Amount)
				.GreaterThan(0).WithMessage("Refund amount must be greater than zero.")
				.LessThanOrEqualTo(1000000).WithMessage("Refund amount cannot exceed 1,000,000.");

			RuleFor(r => r.Reason)
				.NotEmpty().WithMessage("Refund reason is required.")
				.MinimumLength(10).WithMessage("Refund reason must be at least 10 characters.")
				.MaximumLength(1000).WithMessage("Refund reason cannot exceed 1000 characters.");

			RuleFor(r => r.InitiatedBy)
				.NotEmpty().WithMessage("Initiated by user ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Initiated by user ID cannot be empty GUID.");
		}
	}
}
