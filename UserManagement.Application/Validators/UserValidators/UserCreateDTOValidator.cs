using Core.SharedKernel.DTOs;
using FluentValidation;

namespace UserManagement.Application.Validators.UserValidators
{
	public class UserCreateDTOValidator : AbstractValidator<UserCreateDTO>
	{
		public UserCreateDTOValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required")
			.EmailAddress().WithMessage("Invalid email address");
			
		RuleFor(x => x.Password)
			.NotNull().WithMessage("Password is required")
			.MinimumLength(12).WithMessage("Password must be at least 12 characters") // Increased from 8
			.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
			.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
			.Matches("[0-9]").WithMessage("Password must contain at least one number")
			.Matches(@"[@$!%*?&##^()_+\-=\[\]{};':""\\|,.<>\/]").WithMessage("Password must contain at least one special character")
			.Must(NotBeCommonPassword).WithMessage("Password is too common. Please choose a more secure password.");
			
		RuleFor(x => x.ConfirmPassword)
			.NotNull()
			.Equal(x => x.Password).WithMessage("Passwords do not match");
			
		RuleFor(x => x.AgreeToTerms)
			.Equal(true).WithMessage("You must agree to the terms");
	}

	/// <summary>
	/// Checks if password is in list of commonly used weak passwords
		/// </summary>
	private static bool NotBeCommonPassword(string password)
	{
		if (string.IsNullOrWhiteSpace(password))
			return true; // Let NotNull handle this

		// List of top common passwords to reject
		var commonPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"password", "password123", "password1234", "123456", "12345678", 
			"123456789", "1234567890", "qwerty", "abc123", "monkey", 
			"1234567", "letmein", "trustno1", "dragon", "baseball", 
			"iloveyou", "master", "sunshine", "ashley", "bailey", 
			"passw0rd", "shadow", "123123", "654321", "superman", 
			"qazwsx", "michael", "football", "welcome", "administrator",
			"admin", "root", "user", "pass", "1q2w3e4r", "zxcvbnm",
			"asdfghjkl", "qwertyuiop", "123qwe", "password!", "password1"
		};

		return !commonPasswords.Contains(password);
	}
	}
}
