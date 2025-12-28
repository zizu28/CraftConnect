using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using FluentValidation;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Commands.NotificationCommands;
using NotificationManagement.Application.Validators;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Application.CQRS.Handlers.CommandHandlers.NotificationCommandHandlers;

public class SendNotificationCommandHandler(
	IMapper mapper,
	ILoggingService<SendNotificationCommandHandler> logger,
	INotificationRepository notificationRepository,
	INotificationTemplateRepository templateRepository,
	INotificationPreferenceRepository preferenceRepository,
	INotificationProvider notificationProvider,
	IUnitOfWork unitOfWork) : IRequestHandler<SendNotificationCommand, NotificationResponseDTO>
{
	public async Task<NotificationResponseDTO> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
	{
		// Validate request
		var validator = new NotificationCreateDTOValidator();
		var validationResult = await validator.ValidateAsync(request.Notification!, cancellationToken);
		
		if (!validationResult.IsValid)
		{
			logger.LogWarning("Notification validation failed: {Errors}", 
				string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
			throw new ValidationException(validationResult.Errors.First().ErrorMessage);
		}

		Notification notification;
		
		// Check if using template
		if (request.Notification!.TemplateId.HasValue)
		{
			notification = await CreateFromTemplateAsync(request.Notification, cancellationToken);
		}
		else
		{
			// Create directly from DTO
			notification = Notification.Create(
				request.Notification.RecipientId,
				request.Notification.RecipientEmail,
				request.Notification.Type,
				request.Notification.Channel,
				request.Notification.Subject,
				request.Notification.Body,
				request.Notification.Priority,
				templateId: null,
				request.Notification.Metadata
			);
		}

		// Schedule if requested
		if (request.Notification.ScheduledFor.HasValue)
		{
			notification.Schedule(request.Notification.ScheduledFor.Value);
		}

		// Save to repository
		await unitOfWork.ExecuteInTransactionAsync(async () => 
		{
			await notificationRepository.AddAsync(notification, cancellationToken);
		}, cancellationToken);

		logger.LogInformation("Notification {NotificationId} created for user {UserId}", 
			notification.Id, notification.RecipientId);

		// If not scheduled, send immediately
		if (!request.Notification.ScheduledFor.HasValue)
		{
			await SendNotificationAsync(notification, cancellationToken);
		}

		return mapper.Map<NotificationResponseDTO>(notification);
	}

	private async Task<Notification> CreateFromTemplateAsync(
		NotificationCreateDTO dto, 
		CancellationToken cancellationToken)
	{
		var template = await templateRepository.GetByIdAsync(dto.TemplateId!.Value, cancellationToken) ?? throw new KeyNotFoundException($"Template with ID {dto.TemplateId} not found");
		if (!template.IsActive)
			throw new InvalidOperationException($"Template {template.Code} is not active");

		// Render template with variables
		var (subject, body) = template.Render(dto.TemplateVariables ?? []);

		return Notification.Create(
			dto.RecipientId,
			dto.RecipientEmail,
			template.Type,
			template.Channel,
			subject,
			body,
			template.DefaultPriority,
			template.Id,
			dto.Metadata
		);
	}

	private async Task SendNotificationAsync(Notification notification, CancellationToken cancellationToken)
	{
		try
		{
			// Check user preferences before sending
			var preference = await preferenceRepository.GetByUserAndTypeAsync(
				notification.RecipientId, 
				notification.Type, 
				cancellationToken);

			if (preference != null)
			{
				// Check if channel is enabled
				if (!preference.IsChannelEnabled(notification.Channel))
				{
					logger.LogInformation("Notification {NotificationId} not sent - channel {Channel} disabled for user {UserId}",
						notification.Id, notification.Channel, notification.RecipientId);
					notification.Cancel("User has disabled this notification channel");
					await unitOfWork.SaveChangesAsync(cancellationToken);
					return;
				}

				// Check quiet hours (only for non-urgent notifications)
				if (notification.Priority != NotificationPriority.Urgent && preference.IsInQuietHours())
				{
					logger.LogInformation("Notification {NotificationId} not sent - within quiet hours for user {UserId}",
						notification.Id, notification.RecipientId);
					
					// Reschedule for after quiet hours
					var nextSendTime = DateTime.Today.AddDays(1).Add(preference.QuietHoursEnd!.Value.ToTimeSpan());
					notification.Schedule(nextSendTime);
					await unitOfWork.SaveChangesAsync(cancellationToken);
					return;
				}
			}

			notification.MarkAsSending();
			await unitOfWork.SaveChangesAsync(cancellationToken);

			// Get recipient based on channel
			var recipient = notification.Channel == NotificationChannel.Email 
				? notification.RecipientEmail 
				: notification.RecipientPhone;

			if (string.IsNullOrEmpty(recipient))
			{
				notification.MarkAsFailed("No recipient address provided for channel");
				await unitOfWork.SaveChangesAsync(cancellationToken);
				return;
			}

			// Send via provider
			var (success, externalId, errorMessage) = await notificationProvider.SendAsync(
				notification.Channel,
				recipient,
				notification.Subject,
				notification.Body,
				cancellationToken: cancellationToken
			);

			if (success)
			{
				notification.MarkAsSent(provider: "EmailProvider", externalId);
				logger.LogInformation("Notification {NotificationId} sent successfully", notification.Id);
			}
			else
			{
				var canRetry = notification.MarkAsFailed(errorMessage ?? "Unknown error");
				logger.LogWarning("Notification {NotificationId} failed. CanRetry: {CanRetry}", 
					notification.Id, canRetry);
			}

			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error sending notification {NotificationId}", notification.Id);
			notification.MarkAsFailed(ex.Message);
			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
