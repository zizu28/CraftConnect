using Core.SharedKernel.DTOs;
using FluentValidation;

namespace CraftConnect.WASM.Validators
{
	public class CustomerProfileUpdateValidator : AbstractValidator<CustomerUpdateDTO>
	{
		public CustomerProfileUpdateValidator()
		{
			// 1. Identity
			RuleFor(x => x.CustomerId)
				.NotEmpty().WithMessage("Customer ID is missing.");

			// 2. Names
			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First Name is required.")
				.MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last Name is required.")
				.MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.");

			// 3. Contact Info
			RuleFor(x => x.Phone)
				.NotEmpty().WithMessage("Phone number is required.")
				// Basic regex for international phone numbers (allows +, spaces, dashes)
				.Matches(@"^[0-9+\(\)#\.\s\/ext-]+$").WithMessage("Please enter a valid phone number.");

			// 4. Bio
			RuleFor(x => x.Bio)
				.MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.");

			// 5. Address Validation
			// We assume if one address field is filled, the others should be valid too.
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