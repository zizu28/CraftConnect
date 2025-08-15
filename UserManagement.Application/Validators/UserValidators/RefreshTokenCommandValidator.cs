using FluentValidation;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.Validators.UserValidators
{
	public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
	{
		public RefreshTokenCommandValidator()
		{
			RuleFor(x => x.RefreshToken)
				.NotEmpty().WithMessage("Refresh token is required.")
				.Length(10, 100).WithMessage("Refresh token must be between 10 and 100 characters.");
		}
	}
}
