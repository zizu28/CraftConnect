using Core.SharedKernel.DTOs;
using FluentValidation;

namespace UserManagement.Application.Validators.CraftmanValidators
{
	public class CraftmanCreateDTOValidator : AbstractValidator<CraftmanCreateDTO>
	{
		public CraftmanCreateDTOValidator()
		{
			// Base user rules inlined — Include(UserCreateDTOValidator) caused a runtime
			// InvalidOperationException because Include() requires the same generic type.
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email address");

			RuleFor(x => x.Password)
				.NotNull().WithMessage("Password is required")
				.MinimumLength(12).WithMessage("Password must be at least 12 characters")
				.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
				.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
				.Matches("[0-9]").WithMessage("Password must contain at least one number")
				.Matches(@"[@$!%*?&#^()_+\-=\[\]{};':""\\|,.<>\/]")
					.WithMessage("Password must contain at least one special character");

			RuleFor(x => x.ConfirmPassword)
				.NotNull()
				.Equal(x => x.Password).WithMessage("Passwords do not match");

			RuleFor(x => x.AgreeToTerms)
				.Equal(true).WithMessage("You must agree to the terms");

			// Craftman-specific role rule — case-insensitive so "craftman" / "Craftman" both pass
			RuleFor(dto => dto.Role)
				.NotEmpty().WithMessage("Role must not be empty")
				.Must(r => string.Equals(r, "Craftman", StringComparison.OrdinalIgnoreCase))
				.WithMessage("Role must be 'Craftman'");
		}
	}
}
