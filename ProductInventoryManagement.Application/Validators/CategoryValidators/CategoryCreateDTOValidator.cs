using FluentValidation;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;

namespace ProductInventoryManagement.Application.Validators.CategoryValidators
{
	public class CategoryCreateDTOValidator : AbstractValidator<CategoryCreateDTO>
	{
		public CategoryCreateDTOValidator()
		{
			RuleFor(c => c.Name)
				.NotEmpty().WithMessage("Category name is required.")
				.MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.");

			RuleFor(c => c.ParentCategoryId)
				.NotEmpty().WithMessage("Parent Category ID must not be empty.")
				.When(c => c.ParentCategoryId.HasValue);
		}
	}
}