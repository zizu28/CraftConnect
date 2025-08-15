﻿using BookingManagement.Application.DTOs;
using FluentValidation;

namespace BookingManagement.Application.Validators
{
	public class AddressDTOValidator : AbstractValidator<AddressDTO>
	{
		public AddressDTOValidator()
		{
			RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
			RuleFor(x => x.City).NotEmpty().MaximumLength(100);
			RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
		}
	}
}
