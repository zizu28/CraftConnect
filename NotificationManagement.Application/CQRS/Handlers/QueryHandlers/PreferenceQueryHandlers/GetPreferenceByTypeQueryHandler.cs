using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Queries.PreferenceQueries;

namespace NotificationManagement.Application.CQRS.Handlers.QueryHandlers.PreferenceQueryHandlers;

public class GetPreferenceByTypeQueryHandler(
	IMapper mapper,
	INotificationPreferenceRepository preferenceRepository) : IRequestHandler<GetPreferenceByTypeQuery, NotificationPreferenceResponseDTO?>
{
	public async Task<NotificationPreferenceResponseDTO?> Handle(GetPreferenceByTypeQuery request, CancellationToken cancellationToken)
	{
		var preference = await preferenceRepository.GetByUserAndTypeAsync(
			request.UserId, 
			request.NotificationType, 
			cancellationToken);
		
		return preference == null ? null : mapper.Map<NotificationPreferenceResponseDTO>(preference);
	}
}
