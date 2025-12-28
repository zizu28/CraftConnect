using Core.SharedKernel.DTOs;
using MediatR;

namespace NotificationManagement.Application.CQRS.Queries.PreferenceQueries;

public class GetUserPreferencesQuery : IRequest<List<NotificationPreferenceResponseDTO>>
{
	public Guid UserId { get; set; }
}
