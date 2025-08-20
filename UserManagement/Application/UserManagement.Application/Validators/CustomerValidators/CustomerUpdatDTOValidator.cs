﻿using FluentValidation;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Application.Validators.CustomerValidators
{
	public class CustomerUpdatDTOValidator : AbstractValidator<CustomerUpdateDTO>
	{
		public CustomerUpdatDTOValidator()
		{
			When(dto => dto.CustomerId != Guid.Empty, () =>
			{
				RuleFor(dto => dto.CustomerId)
					.NotEmpty().WithMessage("Customer ID is required")
					.Must(id => id != Guid.Empty).WithMessage("Customer ID cannot be empty GUID");
			});

			When(dto => dto.Email != null, () =>
			{
				RuleFor(dto => dto.Email)
					.NotEmpty().WithMessage("Email is required")
					.EmailAddress().WithMessage("Invalid email address");
			});

			When(dto => dto.Password != null, () =>
			{
				RuleFor(dto => dto.Password)
					.NotEmpty().WithMessage("Password is required")
					.MinimumLength(8).WithMessage("Password must be at least 8 characters long");
			});
			When(dto => dto.PreferredPaymentMethod != null, () =>
			{
				RuleFor(dto => dto.PreferredPaymentMethod)
					.NotEmpty().WithMessage("Preferred payment method is required");
			});
		}
	}
}
