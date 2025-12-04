using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;
using FluentValidation;
using UserManagement.Domain.Entities;

namespace CraftConnect.WASM.Validators
{
	public class CraftsmanProfileUpdateValidator : AbstractValidator<CraftsmanProfileUpdateDTO>
	{
		public CraftsmanProfileUpdateValidator(IValidator<Project> projectValidator, 
			IValidator<WorkEntry> workEntryValidator, IValidator<SkillsDTO> skillsDTOValidator)
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

			//RuleForEach(x => x.Portfolio).SetValidator(projectValidator);

			//RuleForEach(x => x.WorkExperience).SetValidator(workEntryValidator);

			//RuleForEach(x => x.Skills).SetValidator(skillsDTOValidator);
		}
	}
}