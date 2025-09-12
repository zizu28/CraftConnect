using AutoMapper;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using PaymentManagement.Application.DTOs.InvoiceDTOS;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.Profiles
{
	public class InvoiceProfile : Profile
	{
		public InvoiceProfile()
		{
			CreateMap<InvoiceCreateDTO, Invoice>()
				.ForPath(dest => dest.BillingAddress, opt => opt.MapFrom(src => new Address(
					src.BillingStreet,
					src.BillingCity,
					src.BillingPostalCode)))
				.ForPath(dest => dest.SubTotal, opt => opt.MapFrom(src => new Money(src.SubTotal, src.Currency)))
				.ForPath(dest => dest.TaxAmount, opt => opt.MapFrom(src => new Money(src.TaxAmount, src.Currency)))
				.ForPath(dest => dest.DiscountAmount, opt => opt.MapFrom(src => new Money(src.DiscountAmount, src.Currency)))
				.ForPath(dest => dest.TotalAmount, opt => opt.MapFrom(src => new Money(src.TotalAmount, src.Currency)))
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<InvoiceStatus>(src.InvoiceStatus)));

			CreateMap<InvoiceUpdateDTO, Invoice>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InvoiceId))
				.ForPath(dest => dest.BillingAddress, opt => opt.MapFrom(src =>
					!string.IsNullOrEmpty(src.BillingStreet) ? new Address(
						src.BillingStreet,
						src.BillingCity ?? string.Empty,
						src.BillingPostalCode ?? string.Empty) : null))
				.ForPath(dest => dest.Status, opt => opt.MapFrom(src => 
					!string.IsNullOrEmpty(src.Status) ? Enum.Parse<InvoiceStatus>(src.Status) : (InvoiceStatus?)null))
				.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<Invoice, InvoiceResponseDTO>()
				.ForPath(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal.Amount))
				.ForPath(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount.Amount))
				.ForPath(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount.Amount))
				.ForPath(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
				.ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
				.ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Recipient))
				.ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.BillingAddress))
				.ForMember(dest => dest.LineItems, opt => opt.MapFrom(src => src.LineItems))
				.ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
				.ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()))
				.ForPath(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.GetOutstandingAmount().Amount))
				.ForMember(dest => dest.DaysUntilDue, opt => opt.MapFrom(src => src.GetDaysUntilDue()));
		}
	}
}
