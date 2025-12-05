using Core.SharedKernel.DTOs;
using FluentValidation;

namespace UserManagement.Application.Validators.CustomerValidators
{
	public class CustomerAddressDTOValidator : AbstractValidator<CustomerAddressDTO>
	{
		public CustomerAddressDTOValidator()
		{
			RuleFor(x => x.Street)
				.NotEmpty().WithMessage("Street is required.")
				.MaximumLength(100).WithMessage("Street cannot exceed 100 characters.");
			RuleFor(x => x.City)
				.NotEmpty().WithMessage("City is required.")
				.MaximumLength(50).WithMessage("City cannot exceed 50 characters.");
			RuleFor(x => x.PostalCode)
				.NotEmpty().WithMessage("Postal code is required.")
				.MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters.");
		}
	}
}
