using FluentValidation;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.Validators.UserValidators
{
	public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
	{
		public ConfirmEmailCommandValidator()
		{
			RuleFor(x => x.token).NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");
		}
	}
}
