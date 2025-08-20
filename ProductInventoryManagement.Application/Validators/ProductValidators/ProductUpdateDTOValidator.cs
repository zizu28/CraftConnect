using FluentValidation;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;

namespace ProductInventoryManagement.Application.Validators.ProductValidators
{
	public class ProductUpdateDTOValidator : AbstractValidator<ProductUpdateDTO>
	{
		public ProductUpdateDTOValidator()
		{
			RuleFor(p => p.Name)
				.MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.")
				.When(p => p.Name != null);

			RuleFor(p => p.Price)
				.GreaterThan(0).WithMessage("Price must be a positive value.")
				.When(p => p.Price.HasValue);

			RuleFor(p => p.CategoryId)
				.NotEmpty().WithMessage("Category ID must not be empty.")
				.When(p => p.CategoryId.HasValue);

			RuleFor(p => p.CraftmanId)
				.NotEmpty().WithMessage("Supplier ID must not be empty.")
				.When(p => p.CraftmanId.HasValue);

			RuleFor(p => p.StockQuantity)
				.GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.")
				.When(p => p.StockQuantity.HasValue);
		}
	}
}