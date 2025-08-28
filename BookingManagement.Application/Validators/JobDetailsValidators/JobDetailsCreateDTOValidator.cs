using BookingManagement.Application.DTOs.JobDetailsDTOs;
using FluentValidation;

namespace BookingManagement.Application.Validators.JobDetailsValidators
{
	public class JobDetailsCreateDTOValidator : AbstractValidator<JobDetailsCreateDTO>
	{
		public JobDetailsCreateDTOValidator()
		{
			RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
			RuleFor(x => x.BookingId)
				.NotEmpty().WithMessage("BookingId is required.")
				.Must(id => id != Guid.Empty).WithMessage("BookingId cannot be an empty GUID.");
		}
	}
}
