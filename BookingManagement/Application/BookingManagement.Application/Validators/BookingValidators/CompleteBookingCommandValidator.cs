using BookingManagement.Application.CQRS.Commands.BookingCommands;
using FluentValidation;

namespace BookingManagement.Application.Validators.BookingValidators
{
	public class CompleteBookingCommandValidator : AbstractValidator<CompleteBookingCommand>
	{
		public CompleteBookingCommandValidator()
		{
			RuleFor(x => x.CompletedAt)
				.NotEmpty().WithMessage("CompletedAt is required.")
				.Must(date => date > DateTime.MinValue).WithMessage("CompletedAt must be a valid date and time.");
			RuleFor(x => x.BookingId)
				.NotEmpty().WithMessage("BookingId is required.")
				.Must(id => id != Guid.Empty).WithMessage("BookingId must be a valid GUID.");
		}
	}
}
