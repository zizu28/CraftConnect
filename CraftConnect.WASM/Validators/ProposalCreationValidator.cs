using CraftConnect.WASM.ViewModels;
using FluentValidation;

namespace CraftConnect.WASM.Validators
{
	public class ProposalCreationValidator : AbstractValidator<ProposalCreationVM>
	{
		public ProposalCreationValidator()
		{
			RuleFor(x => x.CoverLetter)
				.NotEmpty().WithMessage("Cover letter is required.")
				.Length(20, 2000).WithMessage("Please provide between 20 and 2000 characters.");

			RuleFor(x => x.PriceAmount)
				.GreaterThan(0).WithMessage("Proposed price must be greater than zero.");

			RuleFor(x => x.StartDate)
				.NotNull().WithMessage("Start Date is required.");

			RuleFor(x => x.EndDate)
				.NotNull().WithMessage("End Date is required.")
				.GreaterThan(x => x.StartDate).WithMessage("End Date must be after Start Date.");
		}
	}
}