using Core.SharedKernel.DTOs;
using FluentValidation;

namespace NotificationManagement.Application.Validators;

public class NotificationTemplateCreateDTOValidator : AbstractValidator<NotificationTemplateCreateDTO>
{
	public NotificationTemplateCreateDTOValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Template name is required")
			.MaximumLength(100).WithMessage("Template name cannot exceed 100 characters");

		RuleFor(x => x.Code)
			.NotEmpty().WithMessage("Template code is required")
			.MaximumLength(50).WithMessage("Template code cannot exceed 50 characters")
			.Matches("^[A-Z0-9_]+$").WithMessage("Template code must be uppercase alphanumeric with underscores only");

		RuleFor(x => x.Type)
			.IsInEnum().WithMessage("Invalid notification type");

		RuleFor(x => x.Channel)
			.IsInEnum().WithMessage("Invalid notification channel");

		RuleFor(x => x.SubjectTemplate)
			.NotEmpty().WithMessage("Subject template is required")
			.MaximumLength(200).WithMessage("Subject template cannot exceed 200 characters");

		RuleFor(x => x.BodyTemplate)
			.NotEmpty().WithMessage("Body template is required");

		RuleFor(x => x.RequiredVariables)
			.NotNull().WithMessage("Required variables list cannot be null");

		RuleFor(x => x.DefaultMaxRetries)
			.GreaterThanOrEqualTo(0).WithMessage("Max retries cannot be negative")
			.LessThanOrEqualTo(10).WithMessage("Max retries cannot exceed 10");
	}
}
