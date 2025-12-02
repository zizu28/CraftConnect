using Core.SharedKernel.ValueObjects;
using FluentValidation;

namespace CraftConnect.WASM.Validators
{
	public class ProjectValidator : AbstractValidator<Project>
	{
		public ProjectValidator()
		{
			RuleFor(p => p.Title)
				.NotEmpty().WithMessage("Project title is required.")
				.MaximumLength(100).WithMessage("Title is too long.");

			RuleFor(p => p.Description)
				.NotEmpty().WithMessage("Description is required.")
				.MinimumLength(10).WithMessage("Please provide a short description.");

			RuleFor(p => p.ImageUrl).Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
			   .When(p => !string.IsNullOrEmpty(p.ImageUrl))
			   .WithMessage("Invalid Image URL.");
		}
	}
}
