using AutoMapper;
using PaymentManagement.Application.DTOs.PaymentTransactionDTOs;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.Profiles
{
	public class PaymentTransactionProfile : Profile
	{
		public PaymentTransactionProfile()
		{
			CreateMap<PaymentTransaction, PaymentTransactionResponseDTO>()
				.ForPath(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
				.ForPath(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency));
		}
	}
}
