using Core.SharedKernel.DTOs;
using MediatR;

namespace NotificationManagement.Application.CQRS.Commands.PreferenceCommands;

public class CreateOrUpdatePreferenceCommand : IRequest<NotificationPreferenceResponseDTO>
{
	public NotificationPreferenceCreateDTO? Preference { get; set; }
}
