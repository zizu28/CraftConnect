using FluentValidation;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Application.Validators.CustomerValidators
{
	public class CustomerCreateDTOValidator : AbstractValidator<CustomerCreateDTO>
	{
		public CustomerCreateDTOValidator()
		{
			RuleFor(dto => dto.Email)
			.NotEmpty().WithMessage("Email is required")
			.EmailAddress().WithMessage("Invalid email address");

			RuleFor(dto => dto.Password)
				.NotEmpty().WithMessage("Password is required")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters long");

			RuleFor(dto => dto.PreferredPaymentMethod)
				.NotEmpty().WithMessage("Preferred payment method is required");
		}
	}
}
