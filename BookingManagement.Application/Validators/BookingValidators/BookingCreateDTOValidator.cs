using BookingManagement.Application.DTOs.BookingDTOs;
using FluentValidation;
using NodaTime;

namespace BookingManagement.Application.Validators.BookingValidators
{
	public class BookingCreateDTOValidator : AbstractValidator<BookingCreateDTO>
	{
		public BookingCreateDTOValidator()
		{
			RuleFor(x => x.CustomerId)
				.NotEmpty().WithMessage("Customer Id can not be empty.");
			RuleFor(x => x.CraftmanId)
				.NotEmpty().WithMessage("Craftman Id can not be empty.");
			RuleFor(x => x.InitialDescription)
				.NotEmpty().MaximumLength(1000);
			RuleFor(x => x.Street)
				.NotEmpty().WithMessage("Street can not be empty.")
				.MaximumLength(100).WithMessage("Street can not be longer than 100 characters.");
			RuleFor(x => x.City)
				.NotEmpty().WithMessage("City can not be empty.")
				.MaximumLength(100).WithMessage("City can not be longer than 100 characters.");
			RuleFor(x => x.PostalCode)
				.NotEmpty().WithMessage("Postal code can not be empty.")
				.MaximumLength(20).WithMessage("Postal code can not be longer than 20 characters.");
			RuleFor(x => x.Status)
				.NotEmpty().WithMessage("Status can not be empty.")
				.Must(status => status == "Pending" || status == "InProgress" || status == "Completed")
				.WithMessage("Status must be either 'Pending', 'InProgress', or 'Completed'.");
			RuleFor(x => x.StartDate)
				.NotEmpty().WithMessage("Start date can not be empty.")
				.LessThan(x => x.EndDate).WithMessage("Start date must be in the future.");
			RuleFor(x => x.EndDate)
				.NotEmpty().WithMessage("End date can not be empty.")
				.GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");
		}
	}
}
