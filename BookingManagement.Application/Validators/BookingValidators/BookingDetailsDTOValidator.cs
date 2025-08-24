using BookingManagement.Application.DTOs.BookingDTOs;
using FluentValidation;

namespace BookingManagement.Application.Validators.BookingValidators
{
	public class BookingDetailsDTOValidator : AbstractValidator<BookingDetailsDTO>
	{
		public BookingDetailsDTOValidator()
		{
			RuleFor(x => x.Description)
				.NotEmpty()
				.MaximumLength(1000)
				.WithMessage("Description must not be empty and should not exceed 1000 characters.");
		}
	}
}
