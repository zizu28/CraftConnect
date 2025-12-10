using Core.SharedKernel.DTOs;
using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.CQRS.Queries.UserQueries;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
	public class GetBatchCraftsmanSummariesQueryHandler(
		ApplicationDbContext context) : IRequestHandler<GetBatchCraftsmanSummariesQuery, Dictionary<Guid, CraftsmanSummaryDto>>
	{
		private readonly ApplicationDbContext _context = context;

		public async Task<Dictionary<Guid, CraftsmanSummaryDto>> Handle(GetBatchCraftsmanSummariesQuery request, CancellationToken cancellationToken)
		{
			var data = await _context.Craftmen
			   .Where(c => request.Ids.Contains(c.Id))
			   .Select(c => new
			   {
				   c.Id,
				   FullName = c.FirstName + " " + c.LastName,
				   c.ProfileImageUrl
			   })
			   .ToListAsync(cancellationToken);

			var result = data.ToDictionary(
				k => k.Id,
				v => new CraftsmanSummaryDto(v.FullName, v.ProfileImageUrl));
			return result;
		}
	}
}
