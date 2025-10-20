using FluentValidation;
using UserManagement.Domain.Entities;

namespace CraftConnect.WebUI
{
	public partial class ProfileSetupValidator : AbstractValidator<CraftsmanProfileUpdateDTO>
	{
		public ProfileSetupValidator()
		{
			RuleFor(dto => dto.FirstName)
				.NotEmpty().WithMessage("First name is required")
				.MaximumLength(50).WithMessage("First name must not exceed 50 characters");

			RuleFor(dto => dto.LastName)
				.NotEmpty().WithMessage("Last name is required")
				.MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

			RuleFor(dto => dto.Profession)
				.NotEmpty().WithMessage("Profession is required");

			RuleFor(dto => dto.Bio)
				.NotEmpty().WithMessage("Bio is required");
		}
	}
}
