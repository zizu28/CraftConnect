using BookingManagement.Application.DTOs.CraftmanProposalDTOs; // Ensure this matches your namespace
using FluentValidation;

namespace BookingManagement.Application.Validators.CraftsmanProposalValidators
{
	public class UpdateCraftsmanProposalDTOValidator : AbstractValidator<UpdateCraftsmanProposalDTO>
	{
		public UpdateCraftsmanProposalDTOValidator()
		{
			RuleFor(x => x.ProposalId)
				.NotEmpty().WithMessage("Proposal ID is required.");
			RuleFor(x => x.CoverLetter)
				.NotEmpty().WithMessage("Cover letter cannot be empty.")
				.MaximumLength(2000);
			When(x => x.Price != null, () =>
			{
				RuleFor(x => x.Price.Amount).GreaterThan(0).WithMessage("Price must be greater than zero.");
				RuleFor(x => x.Price.Currency).Length(3);
			});
			When(x => x.ProposedTimeline != null, () =>
			{
				RuleFor(x => x.ProposedTimeline.End)
					.GreaterThan(x => x.ProposedTimeline.Start)
					.When(x => x.ProposedTimeline.Start != default && x.ProposedTimeline.End != default)
					.WithMessage("End date must be after start date.");
			});
		}
	}
}