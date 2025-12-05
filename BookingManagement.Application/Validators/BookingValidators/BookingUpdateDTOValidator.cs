using Core.SharedKernel.DTOs;
using FluentValidation;

namespace BookingManagement.Application.Validators.BookingValidators
{
	public class BookingUpdateDTOValidator : AbstractValidator<BookingUpdateDTO>
	{
		public BookingUpdateDTOValidator()
		{
			RuleFor(x => x.BookingId).NotEmpty();
			RuleFor(x => x.NewDescription)
				.NotEmpty()
				.MaximumLength(1000);

			RuleFor(x => x.Status).NotEmpty().WithMessage("Status can not be empty.");

			RuleFor(x => x.PostalCode)
				.NotEmpty()
				.MaximumLength(10)
				.WithMessage("Postal code must not be empty and should not exceed 10 characters.");
			RuleFor(x => x.City)
				.NotEmpty()
				.MaximumLength(100)
				.WithMessage("City must not be empty and should not exceed 100 characters.");
			RuleFor(x => x.Street)
				.NotEmpty()
				.MaximumLength(200)
				.WithMessage("Street must not be empty and should not exceed 200 characters.");
		}
	}
}
