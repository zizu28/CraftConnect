using FluentValidation;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.Validators.UserValidators
{
	public class UserCreateDTOValidator : AbstractValidator<UserCreateDTO>
	{
		public UserCreateDTOValidator()
		{
			RuleFor(user => user.PhoneCountryCode)
				.NotEmpty().WithMessage("Country code is required")
				.Matches(@"^\+\d{1,3}$").WithMessage("Country code must start with '+' and be followed by 1 to 3 digits");

			RuleFor(user => user.PhoneNumber)
				.NotEmpty().WithMessage("Phone number is required")
				.Matches(@"^\d{7,15}$").WithMessage("Phone number must be between 7 and 15 digits");

			RuleFor(user => user.Role)
				.NotNull().WithMessage("Role is required");
				//.IsInEnum().WithMessage("Role must be a valid enum value");
		}
	}
}
