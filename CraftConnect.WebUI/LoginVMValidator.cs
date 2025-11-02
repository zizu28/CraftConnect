using CraftConnect.WebUI.ViewModels;
using FluentValidation;

namespace CraftConnect.WebUI
{
	public class LoginVMValidator : AbstractValidator<LoginUserCommand>
	{
		public LoginVMValidator()
		{
			RuleFor(x => x.Email)
				.NotNull().WithMessage("Please enter your email or username");
			RuleFor(x => x.Password)
				.NotNull().WithMessage("Please enter your password");
		}
	}
}
