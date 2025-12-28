using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using FluentValidation;

namespace NotificationManagement.Application.Validators;

public class NotificationCreateDTOValidator : AbstractValidator<NotificationCreateDTO>
{
	public NotificationCreateDTOValidator()
	{
		RuleFor(x => x.RecipientId)
			.NotEmpty().WithMessage("RecipientId is required");

		RuleFor(x => x.RecipientEmail)
			.NotEmpty().WithMessage("RecipientEmail is required")
			.EmailAddress().WithMessage("Invalid email address")
			.When(x => x.Channel == NotificationChannel.Email);

		RuleFor(x => x.RecipientPhone)
			.NotEmpty().WithMessage("RecipientPhone is required")
			.When(x => x.Channel == NotificationChannel.SMS);

		RuleFor(x => x.Type)
			.IsInEnum().WithMessage("Invalid notification type");

		RuleFor(x => x.Channel)
			.IsInEnum().WithMessage("Invalid notification channel");

		// If not using template, require subject and body
		When(x => !x.TemplateId.HasValue, () =>
		{
			RuleFor(x => x.Subject)
				.NotEmpty().WithMessage("Subject is required when not using template")
				.MaximumLength(200).WithMessage("Subject cannot exceed 200 characters");

			RuleFor(x => x.Body)
				.NotEmpty().WithMessage("Body is required when not using template");
		});

		// If using template, template variables must be provided
		When(x => x.TemplateId.HasValue, () =>
		{
			RuleFor(x => x.TemplateVariables)
				.NotNull().WithMessage("Template variables are required when using template");
		});

		RuleFor(x => x.ScheduledFor)
			.GreaterThan(DateTime.UtcNow).WithMessage("Scheduled time must be in the future")
			.When(x => x.ScheduledFor.HasValue);
	}
}
