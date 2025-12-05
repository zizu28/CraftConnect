using Core.SharedKernel.DTOs;
using FluentValidation;

namespace AuditManagement.Application.Validators
{
	public class CreateAuditLogDtoValidator : AbstractValidator<CreateAuditLogDto>
	{
		public CreateAuditLogDtoValidator()
		{
			RuleFor(x => x.EventType)
				.NotEmpty().WithMessage("Event Type is required.")
				.MaximumLength(100).WithMessage("Event Type cannot exceed 100 characters.");

			RuleFor(x => x.Details)
				.NotEmpty().WithMessage("Details are required.")
				.MaximumLength(1000).WithMessage("Details cannot exceed 1000 characters.");

			RuleFor(x => x.IpAddress)
				.NotEmpty().WithMessage("IP Address is required.")
				// Simple regex for IPv4, can be expanded for IPv6 if needed
				.Matches(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
				.WithMessage("Invalid IP Address format.");

			// Conditional validation: If EntityId is provided, EntityType should also be provided
			RuleFor(x => x.EntityType)
				.NotEmpty().When(x => x.EntityId != Guid.Empty)
				.WithMessage("Entity Type is required when Entity ID is present.");

			// Username is required if UserId is present (to capture snapshot)
			RuleFor(x => x.Username)
				.NotEmpty().When(x => x.UserId != Guid.Empty)
				.WithMessage("Username is required when User ID is present.");
		}
	}
}
