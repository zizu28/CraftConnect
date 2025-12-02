using Core.SharedKernel.Domain;
using FluentValidation;

namespace CraftConnect.WASM.Validators
{
	public class SkillsDTOValidator : AbstractValidator<SkillsDTO>
	{
		public SkillsDTOValidator()
		{
			RuleFor(s => s.Name)
					.NotEmpty().WithMessage("Skill name cannot be empty");

			RuleFor(s => s.YearsOfExperience)
				.GreaterThanOrEqualTo(0).WithMessage("Experience cannot be negative")
				.LessThan(60).WithMessage("Please enter a realistic number of years");
		}
	}
}
