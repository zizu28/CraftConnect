using AutoMapper;
using Core.SharedKernel.DTOs;
using MassTransit.Initializers;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.CraftmanQueries;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers
{
	public class GetCraftmanByProfessionQueryHandler(
		ICraftsmanRepository craftsmanRepository,
		IMapper mapper)
		: IRequestHandler<GetCraftmanByProfessionQuery, CraftmanResponseDTO>
	{
		public async Task<CraftmanResponseDTO> Handle(GetCraftmanByProfessionQuery request, CancellationToken cancellationToken)
		{
			var allCraftsmen = await craftsmanRepository.GetAllAsync(cancellationToken)
				?? throw new NotFoundException($"Craftmen with profession {request.Profession} not found.");

			var craftman = allCraftsmen
				.Where(c => c.Profession.Equals(request.Profession))
				.FirstOrDefault(c => c.Id == request.CraftmanId)
				?? throw new NotFoundException($"Craftman with ID {request.CraftmanId} not found.");

			return mapper.Map<CraftmanResponseDTO>(craftman);
		}
	}
}
