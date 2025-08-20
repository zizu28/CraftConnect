using FluentValidation;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.Validators.UserValidators
{
	public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
	{
		public LoginUserCommandValidator()
		{
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Username is required.");
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
		}
	}
}
