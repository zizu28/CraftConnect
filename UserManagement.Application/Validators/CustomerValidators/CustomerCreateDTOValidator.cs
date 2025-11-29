using FluentValidation;
using UserManagement.Application.DTOs.CustomerDTO;
using UserManagement.Application.Validators.UserValidators;

namespace UserManagement.Application.Validators.CustomerValidators
{
	public class CustomerCreateDTOValidator : AbstractValidator<CustomerCreateDTO>
	{
		public CustomerCreateDTOValidator()
		{
			Include(new UserCreateDTOValidator());
			RuleFor(dto => dto.Role)
				.NotEmpty().WithMessage("Role must not be empty")
				.Equal("Customer").WithMessage("Role must be a customer");
		}
	}
}
