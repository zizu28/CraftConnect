using FluentValidation;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.Validators.CraftmanValidators
{
	public class CraftmanCreateDTOValidator : AbstractValidator<CraftmanCreateDTO>
	{
		public CraftmanCreateDTOValidator()
		{
			RuleFor(dto => dto.FirstName)
				.NotEmpty().WithMessage("First name is required")
				.MaximumLength(50).WithMessage("First name must not exceed 50 characters");
			RuleFor(dto => dto.LastName)
				.NotEmpty().WithMessage("Last name is required")
				.MaximumLength(50).WithMessage("Last name must not exceed 50 characters");
			RuleFor(dto => dto.Email)
			.NotEmpty().WithMessage("Email is required")
			.EmailAddress().WithMessage("Invalid email address");

			RuleFor(dto => dto.Password)
				.NotEmpty().WithMessage("Password is required")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters long");

			RuleFor(dto => dto.Profession)
				.NotEmpty().WithMessage("Profession is required");

			RuleFor(dto => dto.Bio)
				.NotEmpty().WithMessage("Bio is required");

			RuleFor(dto => dto.HourlyRate)
				.GreaterThanOrEqualTo(0).WithMessage("Hourly rate must be greater than or equal to 0");

			RuleFor(dto => dto.Currency)
				.NotEmpty().WithMessage("Currency is required");

			RuleForEach(dto => dto.Skills)
				.Must(skill => !string.IsNullOrEmpty(skill)).WithMessage("Skill name is required");
		}
	}
}
