using AutoMapper;
using Core.SharedKernel.DTOs;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Application.Profiles;

public class NotificationProfileMap : Profile
{
	public NotificationProfileMap()
	{
		// Notification -> NotificationResponseDTO
		CreateMap<Notification, NotificationResponseDTO>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.RecipientId))
			.ForMember(dest => dest.RecipientEmail, opt => opt.MapFrom(src => src.RecipientEmail))
			.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
			.ForMember(dest => dest.Channel, opt => opt.MapFrom(src => src.Channel))
			.ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
			.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
			.ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
			.ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
			.ForMember(dest => dest.RetryCount, opt => opt.MapFrom(src => src.RetryCount))
			.ForMember(dest => dest.MaxRetries, opt => opt.MapFrom(src => src.MaxRetries))
			.ForMember(dest => dest.ScheduledFor, opt => opt.MapFrom(src => src.ScheduledFor))
			.ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt))
			.ForMember(dest => dest.DeliveredAt, opt => opt.MapFrom(src => src.DeliveredAt))
			.ForMember(dest => dest.FailedAt, opt => opt.MapFrom(src => src.FailedAt))
			.ForMember(dest => dest.ErrorMessage, opt => opt.MapFrom(src => src.ErrorMessage))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

		// NotificationTemplate -> NotificationTemplateResponseDTO
		CreateMap<NotificationTemplate, NotificationTemplateResponseDTO>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
			.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
			.ForMember(dest => dest.Channel, opt => opt.MapFrom(src => src.Channel))
			.ForMember(dest => dest.SubjectTemplate, opt => opt.MapFrom(src => src.SubjectTemplate))
			.ForMember(dest => dest.BodyTemplate, opt => opt.MapFrom(src => src.BodyTemplate))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
			.ForMember(dest => dest.DefaultPriority, opt => opt.MapFrom(src => src.DefaultPriority))
			.ForMember(dest => dest.DefaultMaxRetries, opt => opt.MapFrom(src => src.DefaultMaxRetries))
			.ForMember(dest => dest.RequiredVariables, opt => opt.MapFrom(src => src.RequiredVariables))
			.ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

		// NotificationPreference -> NotificationPreferenceResponseDTO
		CreateMap<NotificationPreference, NotificationPreferenceResponseDTO>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
			.ForMember(dest => dest.NotificationType, opt => opt.MapFrom(src => src.NotificationType))
			.ForMember(dest => dest.EmailEnabled, opt => opt.MapFrom(src => src.EmailEnabled))
			.ForMember(dest => dest.SmsEnabled, opt => opt.MapFrom(src => src.SmsEnabled))
			.ForMember(dest => dest.PushEnabled, opt => opt.MapFrom(src => src.PushEnabled))
			.ForMember(dest => dest.InAppEnabled, opt => opt.MapFrom(src => src.InAppEnabled))
			.ForMember(dest => dest.QuietHoursStart, opt => opt.MapFrom(src => src.QuietHoursStart))
			.ForMember(dest => dest.QuietHoursEnd, opt => opt.MapFrom(src => src.QuietHoursEnd))
			.ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
	}
}
