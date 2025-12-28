using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Queries.TemplateQueries;

namespace NotificationManagement.Application.CQRS.Handlers.QueryHandlers.TemplateQueryHandlers;

public class GetTemplateByCodeQueryHandler(
	IMapper mapper,
	INotificationTemplateRepository templateRepository) : IRequestHandler<GetTemplateByCodeQuery, NotificationTemplateResponseDTO?>
{
	public async Task<NotificationTemplateResponseDTO?> Handle(GetTemplateByCodeQuery request, CancellationToken cancellationToken)
	{
		var template = await templateRepository.GetByCodeAsync(request.Code, cancellationToken);
		
		return template == null ? null : mapper.Map<NotificationTemplateResponseDTO>(template);
	}
}
