using BookingManagement.Application.DTOs.CraftmanProposalDTOs; // Ensure this matches your namespace
using FluentValidation;

namespace BookingManagement.Application.Validators.CraftsmanProposalValidators
{
	public class CreateCraftsmanProposalDTOValidator : AbstractValidator<CreateCraftsmanProposalDTO>
	{
		public CreateCraftsmanProposalDTOValidator()
		{
			RuleFor(x => x.ProjectId)
				.NotEmpty().WithMessage("Project ID is required.");
			RuleFor(x => x.CoverLetter)
				.NotEmpty().WithMessage("A cover letter or proposal summary is required.")
				.MinimumLength(20).WithMessage("Please provide a bit more detail in your proposal (min 20 chars).")
				.MaximumLength(2000).WithMessage("Cover letter is too long (max 2000 chars).");
			RuleFor(x => x.Price).NotNull().WithMessage("Price details are required.");
			When(x => x.Price != null, () =>
			{
				RuleFor(x => x.Price.Amount)
					.GreaterThan(0).WithMessage("Price must be greater than zero.");
				RuleFor(x => x.Price.Currency)
					.NotEmpty().WithMessage("Currency is required.")
					.Length(3).WithMessage("Currency must be a 3-letter code (e.g., USD).");
			});
			RuleFor(x => x.ProposedTimeline).NotNull().WithMessage("Timeline is required.");
			When(x => x.ProposedTimeline != null, () =>
			{
				RuleFor(x => x.ProposedTimeline.Start)
					.NotEmpty().WithMessage("Start date is required.")
					.GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past.");
				RuleFor(x => x.ProposedTimeline.End)
					.NotEmpty().WithMessage("End date is required.")
					.GreaterThan(x => x.ProposedTimeline.Start).WithMessage("End date must be after start date.");
			});
		}
	}
}