using Core.SharedKernel.DTOs;
using FluentValidation;

namespace BookingManagement.Application.Validators.CustomerProjectValidators
{
	public class CreateCustomerProjectDTOValidator : AbstractValidator<CreateCustomerProjectDTO>
	{
		public CreateCustomerProjectDTOValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Project title is required.")
				.MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
			RuleFor(x => x.Description)
				.NotEmpty().WithMessage("Project description is required.")
				.MinimumLength(20).WithMessage("Please provide more details about the project (min 20 chars).");
			RuleFor(x => x.Budget).NotNull().WithMessage("Budget is required.");
			When(x => x.Budget != null, () =>
			{
				RuleFor(x => x.Budget.Amount)
					.GreaterThan(0).WithMessage("Budget must be greater than zero.");

				RuleFor(x => x.Budget.Currency)
					.NotEmpty().Length(3).WithMessage("Currency code is required (e.g., USD).");
			});
			RuleFor(x => x.Timeline).NotNull().WithMessage("Timeline is required.");
			When(x => x.Timeline != null, () =>
			{
				RuleFor(x => x.Timeline.Start)
					.GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past.");
				RuleFor(x => x.Timeline.End)
					.GreaterThan(x => x.Timeline.Start).WithMessage("End date must be after the start date.");
			});
			RuleFor(x => x.RequiredSkills)
				.NotEmpty().WithMessage("At least one required skill must be listed.");
			RuleForEach(x => x.RequiredSkills).ChildRules(skill =>
			{
				skill.RuleFor(s => s.Name).NotEmpty().WithMessage("Skill name is required.");
				skill.RuleFor(s => s.YearsOfExperience).GreaterThanOrEqualTo(0);
			});
		}
	}
}