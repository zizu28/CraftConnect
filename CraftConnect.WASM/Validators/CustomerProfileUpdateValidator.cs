using Core.SharedKernel.DTOs;
using FluentValidation;

namespace CraftConnect.WASM.Validators
{
	public class CustomerProfileUpdateValidator : AbstractValidator<CustomerUpdateDTO>
	{
		public CustomerProfileUpdateValidator()
		{
			//RuleFor(x => x.CustomerId)
			//	.NotEmpty().WithMessage("Customer ID is missing.");

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First Name is required.")
				.MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last Name is required.")
				.MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.");

			RuleFor(x => x.Phone)
				.NotEmpty().WithMessage("Phone number is required.")
				.Matches(@"^[0-9+\(\)#\.\s\/ext-]+$").WithMessage("Please enter a valid phone number.");

			RuleFor(x => x.Bio)
				.MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.");

			RuleFor(x => x.Street)
				.NotEmpty().WithMessage("Street address is required.")
				.MaximumLength(100);

			RuleFor(x => x.City)
				.NotEmpty().WithMessage("City is required.")
				.MaximumLength(50);

			RuleFor(x => x.PostalCode)
				.NotEmpty().WithMessage("Zip/Postal Code is required.")
				.MaximumLength(20);
		}
	}
}