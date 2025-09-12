using AutoMapper;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using PaymentManagement.Application.DTOs.PaymentDTOs;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.Profiles
{
	public class PaymentProfile : Profile
	{
		public PaymentProfile()
		{
			CreateMap<PaymentCreateDTO, Payment>()
				.ForPath(dest => dest.Amount, opt => opt.MapFrom(src => new Money(src.Amount, src.Currency)))
				.ForPath(dest => dest.PaymentMethod, opt => opt.MapFrom(src => Enum.Parse<PaymentMethod>(src.PaymentMethod, true)))
				.ForPath(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<PaymentStatus>(src.PaymentStatus, true)))
				.ForPath(dest => dest.PaymentType, opt => opt.MapFrom(src => Enum.Parse<PaymentType>(src.PaymentType, true)))
				.ForPath(dest => dest.BillingAddress, opt => opt.MapFrom(src => new Address(
					src.BillingStreet,
					src.BillingCity,
					src.BillingPostalCode)))
				.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<PaymentUpdateDTO, Payment>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PaymentId))
				.ForPath(dest => dest.BillingAddress, opt => opt.MapFrom(src =>
					!string.IsNullOrEmpty(src.BillingStreet) ? new Address(
						src.BillingStreet,
						src.BillingCity ?? string.Empty,
						src.BillingPostalCode ?? string.Empty) : null))
				.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<Payment, PaymentResponseDTO>()
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
				.ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
				.ForMember(dest => dest.BillingStreet, opt => opt.MapFrom(src => src.BillingAddress.Street))
				.ForMember(dest => dest.BillingCity, opt => opt.MapFrom(src => src.BillingAddress.City))
				.ForMember(dest => dest.BillingPostalCode, opt => opt.MapFrom(src => src.BillingAddress.PostalCode))
				.ForMember(dest => dest.AvailableRefundAmount, opt => opt.MapFrom(src => src.GetAvailableRefundAmount().Amount))
				.ForMember(dest => dest.CanBeRefunded, opt => opt.MapFrom(src => src.CanBeRefunded()))
				.ForMember(dest => dest.Refunds, opt => opt.MapFrom(src => src.Refunds))
				.ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions))
				.ForMember(dest => dest.IsSuccess, opt => opt.MapFrom(src => true))
				.ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Payment retrieved successfully"))
				.ForMember(dest => dest.Errors, opt => opt.MapFrom(src => new List<string>()));
		}
	}
}
