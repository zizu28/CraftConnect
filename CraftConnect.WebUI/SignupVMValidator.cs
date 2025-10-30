using CraftConnect.WebUI.Enums;
using CraftConnect.WebUI.ViewModels;
using FluentValidation;

namespace CraftConnect.WebUI
{
	public class SignupVMValidator : AbstractValidator<UserCreateDTO>
	{
		public SignupVMValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email address");
			RuleFor(x => x.Password)
				.NotNull().WithMessage("Password is required")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters")
				.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$").WithMessage("Password must have at least 8 characters, 1 uppercase, " +
				"1 lowercase, 1 special character and 1 number");
			RuleFor(x => x.ConfirmPassword)
				.NotNull()
				.Equal(x => x.Password).WithMessage("Passwords do not match");
			RuleFor(x => x.AgreeToTerms)
				.Equal(true).WithMessage("You must agree to the terms");
		}
	}
}
