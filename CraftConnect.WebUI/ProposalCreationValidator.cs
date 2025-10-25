using CraftConnect.WebUI.ViewModels;
using FluentValidation;

namespace CraftConnect.WebUI
{
	public class ProposalCreationValidator : AbstractValidator<ProposalCreationVM>
	{
		public ProposalCreationValidator()
		{
			RuleFor(x => x.Description)
				.NotNull().WithMessage("Description is required.")
				.Length(50, 1000).WithMessage("Description must be at least 50 characters.");
			RuleFor(x => x.RateType)
				.NotNull().WithMessage("Rate type must be selected.");
			RuleFor(x => x.ProposedCost)
				.NotNull().WithMessage("Proposed cost is required.")
				.GreaterThan(0).WithMessage("Cost must be a positive number.");
			RuleFor(x => x.StartDate)
				.NotNull().WithMessage("Start Date is required.");
			RuleFor(x => x.EndDate)
				.NotNull().WithMessage("End Date is required.")
				.GreaterThan(x => x.StartDate).WithMessage("End Date must be after Start Date.");
		}
	}
}
