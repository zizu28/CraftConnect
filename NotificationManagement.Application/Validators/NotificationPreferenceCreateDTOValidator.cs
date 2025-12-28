using Core.SharedKernel.DTOs;
using FluentValidation;

namespace NotificationManagement.Application.Validators;

public class NotificationPreferenceCreateDTOValidator : AbstractValidator<NotificationPreferenceCreateDTO>
{
	public NotificationPreferenceCreateDTOValidator()
	{
		RuleFor(x => x.UserId)
			.NotEmpty().WithMessage("UserId is required");

		RuleFor(x => x.NotificationType)
			.IsInEnum().WithMessage("Invalid notification type");

		RuleFor(x => x.Frequency)
			.IsInEnum().WithMessage("Invalid frequency");

		// At least one channel must be enabled
		RuleFor(x => x)
			.Must(x => x.EmailEnabled || x.SmsEnabled || x.PushEnabled || x.InAppEnabled)
			.WithMessage("At least one notification channel must be enabled");

		// Quiet hours validation
		When(x => x.QuietHoursStart.HasValue || x.QuietHoursEnd.HasValue, () =>
		{
			RuleFor(x => x.QuietHoursStart)
				.NotNull().WithMessage("QuietHoursStart must be set if QuietHoursEnd is set")
				.When(x => x.QuietHoursEnd.HasValue);

			RuleFor(x => x.QuietHoursEnd)
				.NotNull().WithMessage("QuietHoursEnd must be set if QuietHoursStart is set")
				.When(x => x.QuietHoursStart.HasValue);
		});
	}
}
