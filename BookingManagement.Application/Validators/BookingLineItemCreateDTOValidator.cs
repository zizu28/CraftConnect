using Core.SharedKernel.DTOs;
using FluentValidation;

namespace BookingManagement.Application.Validators
{
	public class BookingLineItemCreateDTOValidator : AbstractValidator<BookingLineItemCreateDTO>
	{
		public BookingLineItemCreateDTOValidator()
		{
			RuleFor(x => x.Description)
				.NotEmpty()
				.MaximumLength(250);

			RuleFor(x => x.Price)
				.GreaterThan(0)
				.WithMessage("Price must be a positive value.");

			RuleFor(x => x.Quantity)
				.GreaterThan(0)
				.WithMessage("Quantity must be a positive integer.");
		}
	}
}
