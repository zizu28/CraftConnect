using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Queries.PreferenceQueries;

namespace NotificationManagement.Application.CQRS.Handlers.QueryHandlers.PreferenceQueryHandlers;

public class GetUserPreferencesQueryHandler(
	IMapper mapper,
	INotificationPreferenceRepository preferenceRepository) : IRequestHandler<GetUserPreferencesQuery, List<NotificationPreferenceResponseDTO>>
{
	public async Task<List<NotificationPreferenceResponseDTO>> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
	{
		var preferences = await preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);
		
		return mapper.Map<List<NotificationPreferenceResponseDTO>>(preferences);
	}
}
