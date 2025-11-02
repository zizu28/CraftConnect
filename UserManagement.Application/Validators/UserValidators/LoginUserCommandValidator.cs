using FluentValidation;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.Validators.UserValidators
{
	public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
	{
		public LoginUserCommandValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Email format error.");
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$").WithMessage("Password must have at least 8 characters, 1 uppercase, " +
				"1 lowercase, 1 special character and 1 number");
		}
	}
}
