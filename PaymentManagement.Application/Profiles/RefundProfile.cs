using AutoMapper;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using PaymentManagement.Application.DTOs.RefundDTOs;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.Profiles
{
	public class RefundProfile : Profile
	{
		public RefundProfile()
		{
			CreateMap<RefundCreateDTO, Refund>()
				.ForPath(dest => dest.Amount, opt => opt.MapFrom(src => new Money(src.Amount, src.Currency)))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<RefundStatus>(src.Status)))
				.ForMember(dest => dest.ExternalRefundId, opt => opt.MapFrom(src => src.ExternalRefundId))
				.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<Refund, RefundResponseDTO>()
				.ForMember(dest => dest.RefundId, opt => opt.MapFrom(src => src.Id))
				.ForPath(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
				.ForPath(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency));
		}
	}
}
