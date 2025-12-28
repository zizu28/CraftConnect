using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using FluentValidation;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Commands.PreferenceCommands;
using NotificationManagement.Application.Validators;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Application.CQRS.Handlers.CommandHandlers.PreferenceCommandHandlers;

public class CreateOrUpdatePreferenceCommandHandler(
	IMapper mapper,
	ILoggingService<CreateOrUpdatePreferenceCommandHandler> logger,
	INotificationPreferenceRepository preferenceRepository,
	IUnitOfWork unitOfWork) : IRequestHandler<CreateOrUpdatePreferenceCommand, NotificationPreferenceResponseDTO>
{
	public async Task<NotificationPreferenceResponseDTO> Handle(CreateOrUpdatePreferenceCommand request, CancellationToken cancellationToken)
	{
		// Validate
		var validator = new NotificationPreferenceCreateDTOValidator();
		var validationResult = await validator.ValidateAsync(request.Preference!, cancellationToken);
		
		if (!validationResult.IsValid)
		{
			logger.LogWarning("Preference validation failed: {Errors}", 
				string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
			throw new ValidationException(validationResult.Errors.First().ErrorMessage);
		}

		// Check if preference already exists
		var existing = await preferenceRepository.GetByUserAndTypeAsync(
			request.Preference!.UserId, 
			request.Preference.NotificationType, 
			cancellationToken);

		NotificationPreference preference;

		if (existing == null)
		{
			// Create new preference
			preference = NotificationPreference.CreateDefault(
				request.Preference.UserId, 
				request.Preference.NotificationType);

			// Update with user's choices
			preference.UpdateChannelPreferences(
				request.Preference.EmailEnabled,
				request.Preference.SmsEnabled,
				request.Preference.PushEnabled,
				request.Preference.InAppEnabled);

			if (request.Preference.QuietHoursStart.HasValue && request.Preference.QuietHoursEnd.HasValue)
			{
				preference.SetQuietHours(
					request.Preference.QuietHoursStart.Value,
					request.Preference.QuietHoursEnd.Value);
			}

			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				await preferenceRepository.AddAsync(preference, cancellationToken);
			}, cancellationToken);

			logger.LogInformation("Created preference for user {UserId}, type {Type}", 
				preference.UserId, preference.NotificationType);
		}
		else
		{
			// Update existing preference
			existing.UpdateChannelPreferences(
				request.Preference.EmailEnabled,
				request.Preference.SmsEnabled,
				request.Preference.PushEnabled,
				request.Preference.InAppEnabled);

			if (request.Preference.QuietHoursStart.HasValue && request.Preference.QuietHoursEnd.HasValue)
			{
				existing.SetQuietHours(
					request.Preference.QuietHoursStart.Value,
					request.Preference.QuietHoursEnd.Value);
			}
			else
			{
				existing.ClearQuietHours();
			}

			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				await preferenceRepository.UpdateAsync(existing, cancellationToken);
			}, cancellationToken);

			preference = existing;

			logger.LogInformation("Updated preference for user {UserId}, type {Type}", 
				preference.UserId, preference.NotificationType);
		}

		return mapper.Map<NotificationPreferenceResponseDTO>(preference);
	}
}
