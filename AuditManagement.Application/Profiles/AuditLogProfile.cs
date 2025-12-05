using AuditManagement.Domain.Entities;
using AutoMapper;
using Core.SharedKernel.DTOs;

namespace AuditManagement.Application.Profiles
{
	public class AuditLogProfile : Profile
	{
		public AuditLogProfile()
		{
			CreateMap<CreateAuditLogDto, AuditLog>()
				.ForMember(dest => dest.IpAddress, opt => opt.Ignore()) // IP is a value object in domain, string in DTO
				.ConstructUsing(src => AuditLog.Create(
					src.EventType, src.UserId, src.Details, src.IpAddress, src.EntityId,
					src.OldValue, src.NewValue)
				);

			CreateMap<AuditLog, AuditLogResponseDto>()
				.ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress.ToString()));
		}
	}
}
