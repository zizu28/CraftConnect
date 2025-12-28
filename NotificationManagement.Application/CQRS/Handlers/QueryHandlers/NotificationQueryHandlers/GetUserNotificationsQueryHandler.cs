using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Queries.NotificationQueries;

namespace NotificationManagement.Application.CQRS.Handlers.QueryHandlers.NotificationQueryHandlers;

public class GetUserNotificationsQueryHandler(
	IMapper mapper,
	INotificationRepository notificationRepository) : IRequestHandler<GetUserNotificationsQuery, List<NotificationResponseDTO>>
{
	public async Task<List<NotificationResponseDTO>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
	{
		var notifications = await notificationRepository.GetByUserIdAsync(
			request.UserId, 
			request.PageNumber, 
			request.PageSize, 
			cancellationToken);
		
		return mapper.Map<List<NotificationResponseDTO>>(notifications);
	}
}
