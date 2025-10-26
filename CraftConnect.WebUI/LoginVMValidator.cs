using CraftConnect.WebUI.ViewModels;
using FluentValidation;

namespace CraftConnect.WebUI
{
	public class LoginVMValidator : AbstractValidator<LoginVM>
	{
		public LoginVMValidator()
		{
			RuleFor(x => x.EmailOrUsername)
				.NotNull().WithMessage("Please enter your email or username");
			RuleFor(x => x.Password)
				.NotNull().WithMessage("Please enter your password");
		}
	}
}
