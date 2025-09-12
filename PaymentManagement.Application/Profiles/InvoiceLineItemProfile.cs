using AutoMapper;
using Core.SharedKernel.ValueObjects;
using PaymentManagement.Application.DTOs.InvoiceLineItemDTOs;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.Profiles
{
	public class InvoiceLineItemProfile : Profile
	{
		public InvoiceLineItemProfile()
		{
			CreateMap<InvoiceLineItemCreateDTO, InvoiceLineItem>()
				.ForPath(dest => dest.UnitPrice, opt => opt.MapFrom(src => new Money(src.UnitPrice, src.Currency)))
				.ForPath(dest => dest.TotalPrice, opt => opt.MapFrom(src => new Money((src.UnitPrice * src.Quantity), src.Currency)));

			CreateMap<InvoiceLineItemUpdateDTO, InvoiceLineItem>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LineItemId))
				.ForPath(dest => dest.UnitPrice, opt => opt.MapFrom(src =>
					src.UnitPrice.HasValue ? new Money(src.UnitPrice.Value, src.Currency!) : null));

			CreateMap<InvoiceLineItem, InvoiceLineItemResponseDTO>()
				.ForPath(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
				.ForPath(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice.Amount))
				.ForPath(dest => dest.Currency, opt => opt.MapFrom(src => src.UnitPrice.Currency));
		}
	}
}
