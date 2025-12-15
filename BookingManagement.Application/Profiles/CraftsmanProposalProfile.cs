using AutoMapper;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.ValueObjects;

namespace BookingManagement.Application.Profiles
{
	public class CraftsmanProposalProfile : Profile
	{
		public CraftsmanProposalProfile()
		{
			CreateMap<CraftsmanProposal, CraftsmanProposalResponseDTO>()
				.ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
					new MoneyDTO { Amount = src.Price.Amount, Currency = src.Price.Currency }))
				.ForMember(dest => dest.ProposedTimeline, opt => opt.MapFrom(src =>
					new DateTimeRangeDTO { Start = src.ProposedTimeline.Start, End = src.ProposedTimeline.End }))
				.ForMember(dest => dest.CraftsmanName, opt => opt.Ignore());


			CreateMap<CreateCraftsmanProposalDTO, CraftsmanProposal>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
					new Money(src.Price.Amount, src.Price.Currency)))
				.ForMember(dest => dest.ProposedTimeline, opt => opt.MapFrom(src =>
					new DateTimeRange(src.ProposedTimeline.Start, src.ProposedTimeline.End)));


			CreateMap<UpdateCraftsmanProposalDTO, CraftsmanProposal>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.ProjectId, opt => opt.Ignore())
				.ForMember(dest => dest.CraftsmanId, opt => opt.Ignore())
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
					new Money(src.Price.Amount, src.Price.Currency)))
				.ForMember(dest => dest.ProposedTimeline, opt => opt.MapFrom(src =>
					new DateTimeRange(src.ProposedTimeline.Start, src.ProposedTimeline.End)));
		}
	}
}