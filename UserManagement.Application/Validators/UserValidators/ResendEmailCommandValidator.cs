using FluentValidation;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.Validators.UserValidators
{
	public class ResendEmailCommandValidator : AbstractValidator<ResendEmailCommand>
	{
		public ResendEmailCommandValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format");
		}
	}
}
