using Core.SharedKernel.ValueObjects;
using FluentValidation;

namespace CraftConnect.WASM.Validators
{
	public class WorkEntryValidator : AbstractValidator<WorkEntry>
	{
		public WorkEntryValidator()
		{
			RuleFor(w => w.StartDate)
					.NotEmpty().WithMessage("Start date is required");

			RuleFor(w => w.EndDate)
				.Must((entry, endDate) => endDate > entry.StartDate)
				.When(w => w.EndDate > w.StartDate, ApplyConditionTo.CurrentValidator)
				.WithMessage("End date must be after Start date");
			RuleFor(w => w.Company)
				.NotEmpty().WithMessage("Company name is required.");

			RuleFor(w => w.Position)
				.NotEmpty().WithMessage("Position is required.");
			RuleFor(w => w.Responsibilities)
				.NotEmpty().WithMessage("Responsibilities are required.");
		}
	}
}
