using CraftConnect.WebUI.ViewModels;
using FluentValidation;

namespace CraftConnect.WebUI
{
	public class SignupVMValidator : AbstractValidator<SignupVM>
	{
		public SignupVMValidator()
		{
			RuleFor(x => x.EmailAddress)
				.NotNull().WithMessage("Email is required")
				.EmailAddress();
			RuleFor(x => x.Password)
				.NotNull().WithMessage("Password is required")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters")
				.Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$").WithMessage("Password must have at least 8 characters, 1 uppercase, and 1 number");
			RuleFor(x => x.ConfirmPassword)
				.NotNull()
				.Matches(x => x.Password).WithMessage("Passwords do not match");
			RuleFor(x => x.AgreeToTerms)
				.NotNull().WithMessage("You must agree to the terms");

		}
	}
}
