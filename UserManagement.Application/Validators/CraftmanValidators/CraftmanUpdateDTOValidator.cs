using FluentValidation;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Validators.CraftmanValidators
{
	public class CraftmanUpdateDTOValidator : AbstractValidator<CraftsmanProfileUpdateDTO>
	{
		public CraftmanUpdateDTOValidator()
		{
			When(dto => dto.CraftmanId != Guid.Empty, () =>
			{
				RuleFor(dto => dto.CraftmanId)
					.NotNull().WithMessage("Invalid Craftman ID");
			});
			When(dto => dto.FirstName != null, () =>
			{
				RuleFor(dto => dto.FirstName)
					.NotNull().WithMessage("First name is required");
			});
			When(x => x.LastName != null, () =>
			{
				RuleFor(x => x.LastName)
					.NotNull().WithMessage("Last name is required");
			});
			When(dto => dto.Bio != null, () =>
			{
				RuleFor(dto => dto.Bio)
					.NotEmpty().WithMessage("Bio is required");
			});
			When(x => x.Profession.ToString() != null, () =>
			{
				RuleFor(x => x.Profession)
					.IsInEnum()
					.NotNull().WithMessage("Profession is required");
			});
			When(dto => dto.Portfolio != null, () =>
			{
				RuleForEach(x => x.Portfolio)
					.Must(portfolio => !string.IsNullOrEmpty(portfolio.Title)).WithMessage("Portfolio title is required")
					.Must(x => !string.IsNullOrEmpty(x.Description)).WithMessage("Portfolio description is required");
			});
			When(dto => dto.WorkExperience != null, () =>
			{
				RuleForEach(dto => dto.WorkExperience)
					.Must(x => x.StartDate != null)
					.Must(x => x.EndDate > x.StartDate).WithMessage("The end date should be later than the start date");
			});
			When(dto => dto.Skills != null, () =>
			{
				RuleForEach(dto => dto.Skills)
					.Must(skill => !string.IsNullOrEmpty(skill.Name)).WithMessage("Skill name is required")
					.Must(skill => skill.YearsOfExperience >= 0).WithMessage("Years of experience cannot be less than 0");
			});	
		}
	}
}
