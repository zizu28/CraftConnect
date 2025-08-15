using FluentValidation;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.Validators.UserValidators
{
	public class UserUpdateDTOValidator : AbstractValidator<UserUpdateDTO>
	{
		public UserUpdateDTOValidator()
		{
			When(user => user.UserId != Guid.Empty, () =>
			{
				RuleFor(user => user.UserId)
					.NotEmpty().WithMessage("User ID is required")
					.Must(id => id != Guid.Empty).WithMessage("User ID cannot be empty GUID");
			});

			When(user => user.PhoneCountryCode != null, () =>
			{
				RuleFor(user => user.PhoneCountryCode)
					.NotEmpty().WithMessage("Country code is required")
					.Matches(@"^\+\d{1,3}$").WithMessage("Country code must start with '+' and be followed by 1 to 3 digits");
			});
			When(user => user.PhoneNumber != null, () =>
			{
				RuleFor(user => user.PhoneNumber)
					.NotEmpty().WithMessage("Phone number is required")
					.Matches(@"^\d{7,15}$").WithMessage("Phone number must be between 7 and 15 digits");
			});
			When(user => !string.IsNullOrEmpty(user.Role), () =>
			{
				RuleFor(user => user.Role)
					.NotNull().WithMessage("Role is required");
					//.IsInEnum().WithMessage("Role must be a valid enum value");
			});
		}
	}
}
