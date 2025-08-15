using FluentValidation;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.Validators.CraftmanValidators
{
	public class CraftmanUpdateDTOValidator : AbstractValidator<CraftmanUpdateDTO>
	{
		public CraftmanUpdateDTOValidator()
		{
			When(dto => dto.CraftmanId != Guid.Empty, () =>
			{
				RuleFor(dto => dto.CraftmanId)
					.NotNull().WithMessage("Invalid Craftman ID");
			});
			When(dto => dto.Bio != null, () =>
			{
				RuleFor(dto => dto.Bio)
					.NotEmpty().WithMessage("Bio is required");
			});
			When(dto => dto.HourlyRate != 0, () =>
			{
				RuleFor(dto => dto.HourlyRate)
					.GreaterThanOrEqualTo(0).WithMessage("Hourly rate must be greater than or equal to 0");
			});
			When(dto => dto.Currency != null, () =>
			{
				RuleFor(dto => dto.Currency)
					.NotEmpty().WithMessage("Currency is required");
			});
			When(dto => dto.Skills != null, () =>
			{
				RuleForEach(dto => dto.Skills)
					.Must(skill => !string.IsNullOrEmpty(skill)).WithMessage("Skill name is required");
			});	
		}
	}
}
