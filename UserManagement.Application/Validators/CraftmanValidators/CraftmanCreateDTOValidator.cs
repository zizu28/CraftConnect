using FluentValidation;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Application.Validators.UserValidators;

namespace UserManagement.Application.Validators.CraftmanValidators
{
	public class CraftmanCreateDTOValidator : AbstractValidator<CraftmanCreateDTO>
	{
		public CraftmanCreateDTOValidator()
		{
			Include(new UserCreateDTOValidator());
			RuleFor(dto => dto.Role)
				.NotEmpty().WithMessage("Role must not be empty")
				.Equal("Craftman").WithMessage("Role must be a craftperson");
		}
	}
}
