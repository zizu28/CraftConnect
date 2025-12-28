using Core.SharedKernel.DTOs;
using MediatR;

namespace NotificationManagement.Application.CQRS.Queries.TemplateQueries;

public class GetTemplateByCodeQuery : IRequest<NotificationTemplateResponseDTO?>
{
	public string Code { get; set; } = string.Empty;
}
