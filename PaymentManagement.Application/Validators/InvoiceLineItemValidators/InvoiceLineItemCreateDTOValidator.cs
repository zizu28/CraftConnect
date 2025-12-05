using Core.SharedKernel.DTOs;
using FluentValidation;

namespace PaymentManagement.Application.Validators.InvoiceLineItemValidators
{
	public class InvoiceLineItemCreateDTOValidator : AbstractValidator<InvoiceLineItemCreateDTO>
	{
		public InvoiceLineItemCreateDTOValidator()
		{
			RuleFor(li => li.Description)
				.NotEmpty().WithMessage("Line item description is required.")
				.MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

			RuleFor(li => li.UnitPrice)
				.GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.")
				.LessThanOrEqualTo(1000000).WithMessage("Unit price cannot exceed 1,000,000.");

			RuleFor(li => li.Quantity)
				.GreaterThan(0).WithMessage("Quantity must be greater than zero.")
				.LessThanOrEqualTo(10000).WithMessage("Quantity cannot exceed 10,000.");

			When(li => !string.IsNullOrEmpty(li.ItemCode), () =>
			{
				RuleFor(li => li.ItemCode)
					.MaximumLength(100).WithMessage("Item code cannot exceed 100 characters.")
					.Matches(@"^[A-Za-z0-9\-_]+$").WithMessage("Item code can only contain letters, numbers, hyphens, and underscores.");
			});
		}
	}
}
