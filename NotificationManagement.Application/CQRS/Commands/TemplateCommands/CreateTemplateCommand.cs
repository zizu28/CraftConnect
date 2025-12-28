using Core.SharedKernel.DTOs;
using MediatR;

namespace NotificationManagement.Application.CQRS.Commands.TemplateCommands;

public class CreateTemplateCommand : IRequest<NotificationTemplateResponseDTO>
{
	public NotificationTemplateCreateDTO? Template { get; set; }
	public Guid CreatedBy { get; set; }
}
