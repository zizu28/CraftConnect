using Core.SharedKernel.DTOs;
using FluentValidation;

namespace PaymentManagement.Application.Validators.InvoiceLineItemValidators
{
	public class InvoiceLineItemUpdateDTOValidator : AbstractValidator<InvoiceLineItemUpdateDTO>
	{
		public InvoiceLineItemUpdateDTOValidator()
		{
			RuleFor(li => li.LineItemId)
				.NotEmpty().WithMessage("Line item ID is required.")
				.Must(id => id != Guid.Empty).WithMessage("Line item ID cannot be empty GUID.");

			When(li => !string.IsNullOrEmpty(li.Description), () =>
			{
				RuleFor(li => li.Description)
					.MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
			});

			When(li => li.UnitPrice.HasValue, () =>
			{
				RuleFor(li => li.UnitPrice.Value)
					.GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.")
					.LessThanOrEqualTo(1000000).WithMessage("Unit price cannot exceed 1,000,000.");
			});

			When(li => li.Quantity.HasValue, () =>
			{
				RuleFor(li => li.Quantity.Value)
					.GreaterThan(0).WithMessage("Quantity must be greater than zero.")
					.LessThanOrEqualTo(10000).WithMessage("Quantity cannot exceed 10,000.");
			});

			When(li => !string.IsNullOrEmpty(li.ItemCode), () =>
			{
				RuleFor(li => li.ItemCode)
					.MaximumLength(100).WithMessage("Item code cannot exceed 100 characters.")
					.Matches(@"^[A-Za-z0-9\-_]+$").WithMessage("Item code can only contain letters, numbers, hyphens, and underscores.");
			});
		}
	}
}
