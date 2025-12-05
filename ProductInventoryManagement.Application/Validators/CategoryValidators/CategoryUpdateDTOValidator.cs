using Core.SharedKernel.DTOs;
using FluentValidation;

namespace ProductInventoryManagement.Application.Validators.CategoryValidators
{
	public class CategoryUpdateDTOValidator : AbstractValidator<CategoryUpdateDTO>
	{
		public CategoryUpdateDTOValidator()
		{
			RuleFor(c => c.Name)
				.NotEmpty().WithMessage("Category name cannot be empty.")
				.MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.")
				.When(c => c.Name != null);

			RuleFor(c => c.ParentCategoryId)
				.NotEmpty().WithMessage("Parent Category ID must not be empty.")
				.When(c => c.ParentCategoryId.HasValue);
		}
	}
}