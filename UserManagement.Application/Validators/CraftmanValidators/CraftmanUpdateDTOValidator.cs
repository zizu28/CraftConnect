using FluentValidation;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Validators.CraftmanValidators
{
	public class CraftmanUpdateDTOValidator : AbstractValidator<CraftsmanProfileUpdateDTO>
	{
		public CraftmanUpdateDTOValidator()
		{
			RuleFor(x => x.CraftmanId)
				.NotEmpty().WithMessage("Invalid Craftman ID");

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required")
				.MaximumLength(50).WithMessage("First name is too long");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required");

			RuleFor(x => x.Bio)
				.NotEmpty().WithMessage("Bio is required")
				.MinimumLength(20).WithMessage("Please write at least a short sentence.");

			RuleFor(x => x.Profession)
				.NotEmpty().WithMessage("Profession is required");

			RuleForEach(x => x.Portfolio).ChildRules(portfolio =>
			{
				portfolio.RuleFor(p => p.Title)
					.NotEmpty().WithMessage("Project title is required");

				portfolio.RuleFor(p => p.Description)
					.NotEmpty().WithMessage("Project description is required");
			});

			//RuleForEach(x => x.WorkExperience).ChildRules(work =>
			//{
			//	work.RuleFor(w => w.StartDate)
			//		.NotEmpty().WithMessage("Start date is required");

			//	work.RuleFor(w => w.EndDate)
			//		.Must((entry, endDate) => endDate > entry.StartDate)
			//		.When(w => w.EndDate > w.StartDate)
			//		.WithMessage("End date must be after Start date");
			//});

			//RuleForEach(x => x.Skills).ChildRules(skill =>
			//{
			//	skill.RuleFor(s => s.Name)
			//		.NotEmpty().WithMessage("Skill name cannot be empty");

			//	skill.RuleFor(s => s.YearsOfExperience)
			//		.GreaterThanOrEqualTo(0).WithMessage("Experience cannot be negative")
			//		.LessThan(60).WithMessage("Please enter a realistic number of years");
			//});
		}
	}
}
