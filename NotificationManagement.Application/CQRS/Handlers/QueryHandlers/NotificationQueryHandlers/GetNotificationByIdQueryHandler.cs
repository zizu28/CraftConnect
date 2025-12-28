using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Queries.NotificationQueries;

namespace NotificationManagement.Application.CQRS.Handlers.QueryHandlers.NotificationQueryHandlers;

public class GetNotificationByIdQueryHandler(
	IMapper mapper,
	INotificationRepository notificationRepository) : IRequestHandler<GetNotificationByIdQuery, NotificationResponseDTO?>
{
	public async Task<NotificationResponseDTO?> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
	{
		var notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
		
		return notification == null ? null : mapper.Map<NotificationResponseDTO>(notification);
	}
}
