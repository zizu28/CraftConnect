using BookingManagement.Application.CQRS.Commands.BookingCommands;
using FluentValidation;

namespace BookingManagement.Application.Validators.BookingValidators
{
	public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
	{
		public CreateBookingCommandValidator()
		{
			RuleFor(x => x.BookingDTO.CustomerId)
				.NotEmpty().WithMessage("CustomerId is required.")
				.Must(id => id != Guid.Empty).WithMessage("CustomerId must be a valid GUID.");
			RuleFor(x => x.BookingDTO.CraftmanId)
				.NotEmpty().WithMessage("CraftmanId is required.")
				.Must(id => id != Guid.Empty).WithMessage("CraftmanId must be a valid GUID.");
			RuleFor(x => x.BookingDTO.StartDate)
				.NotEmpty().WithMessage("StartDate is required.")
				.Must(date => date > DateTime.MinValue).WithMessage("StartDate must be a valid date and time.");
			RuleFor(x => x.BookingDTO.EndDate)
				.NotEmpty().WithMessage("EndDate is required.")
				.Must(date => date > DateTime.MinValue).WithMessage("EndDate must be a valid date and time.")
				.GreaterThan(x => x.BookingDTO.StartDate).WithMessage("EndDate must be after StartDate.");
			RuleFor(x => x.BookingDTO.Status)
				.NotEmpty().WithMessage("Status is required.")
				.IsInEnum().WithMessage("Status must be a valid BookingStatus enum value.");
			RuleFor(x => x.BookingDTO.InitialDescription)
				.NotEmpty().WithMessage("InitialDescription is required.")
				.MaximumLength(500).WithMessage("InitialDescription must not exceed 500 characters.");
			RuleFor(x => x.BookingDTO.City)
				.NotEmpty().WithMessage("City is required.")
				.MaximumLength(100).WithMessage("City must not exceed 100 characters.");
			RuleFor(x => x.BookingDTO.PostalCode)
				.NotEmpty().WithMessage("PostalCode is required.")
				.MaximumLength(20).WithMessage("PostalCode must not exceed 20 characters.");
			RuleFor(x => x.BookingDTO.Street)
				.NotEmpty().WithMessage("Street is required.")
				.MaximumLength(200).WithMessage("Street must not exceed 200 characters.");
		}
	}
}
