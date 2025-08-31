using FluentValidation;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;

namespace ProductInventoryManagement.Application.Validators.ProductValidators
{
	public class ProductCreateDTOValidator : AbstractValidator<ProductCreateDTO>
	{
		public ProductCreateDTOValidator()
		{
			RuleFor(p => p.Name)
				.NotEmpty().WithMessage("Product name is required.")
				.MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

			RuleFor(p => p.Description)
				.NotEmpty().WithMessage("Product description is required.");

			RuleFor(p => p.Price)
				.GreaterThan(0).WithMessage("Price must be a positive value.");

			RuleFor(p => p.CategoryId)
				.NotEmpty().WithMessage("Category ID is required and must not be empty.");

			RuleFor(p => p.CraftmanId)
				.NotEmpty().WithMessage("Supplier ID is required and must not be empty.");

			RuleFor(p => p.StockQuantity)
				.GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
		}
	}
}