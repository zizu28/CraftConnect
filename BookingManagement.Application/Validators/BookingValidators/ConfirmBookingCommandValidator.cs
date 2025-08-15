using BookingManagement.Application.CQRS.Commands.BookingCommands;
using FluentValidation;

namespace BookingManagement.Application.Validators.BookingValidators
{
	public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
	{
		public ConfirmBookingCommandValidator()
		{
			RuleFor(x => x.ConfirmedAt)
				.NotEmpty().WithMessage("ConfirmedAt is required.")
				.Must(date => date > DateTime.MinValue).WithMessage("ConfirmedAt must be a valid date and time.");
			RuleFor(x => x.BookingId)
				.NotEmpty().WithMessage("BookingId is required.")
				.Must(id => id != Guid.Empty).WithMessage("BookingId must be a valid GUID.");
			RuleFor(x => x.CraftmanId)
				.NotEmpty().WithMessage("CraftmanId is required.")
				.Must(id => id != Guid.Empty).WithMessage("CraftmanId must be a valid GUID.");
			RuleFor(x => x.CustomerId)
				.NotEmpty().WithMessage("CustomerId is required.")
				.Must(id => id != Guid.Empty).WithMessage("CustomerId must be a valid GUID.");
		}
	}
}
