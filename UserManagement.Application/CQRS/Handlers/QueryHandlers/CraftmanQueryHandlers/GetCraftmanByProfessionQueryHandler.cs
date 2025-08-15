using AutoMapper;
using MassTransit.Initializers;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.CraftmanQueries;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers
{
	public class GetCraftmanByProfessionQueryHandler(
		ICraftsmanRepository craftsmanRepository, IMapper mapper)
		: IRequestHandler<GetCraftmanByProfessionQuery, CraftmanResponseDTO>
	{
		public async Task<CraftmanResponseDTO> Handle(GetCraftmanByProfessionQuery request, CancellationToken cancellationToken)
		{
			var craftmenProfessions = await craftsmanRepository.GetAllAsync(cancellationToken)
				.Select(craftman => craftman.Where(profession => profession.Profession.Equals(request.Profession)))
				?? throw new NotFoundException($"Craftmen with profession {request.Profession} not found.");
			
			var craftman = craftmenProfessions.FirstOrDefault(c => c.Id == request.CraftmanId)
				?? throw new NotFoundException($"Craftman with ID {request.CraftmanId} not found.");
			return mapper.Map<CraftmanResponseDTO>(craftman);
		}
	}
}
