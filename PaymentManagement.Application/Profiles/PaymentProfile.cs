using AutoMapper;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using PaymentManagement.Application.DTOs.InvoiceDTOS;
using PaymentManagement.Application.DTOs.InvoiceLineItemDTOs;
using PaymentManagement.Application.DTOs.PaymentDTOs;
using PaymentManagement.Application.DTOs.PaymentTransactionDTOs;
using PaymentManagement.Application.DTOs.RefundDTOs;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.Profiles
{
	public class PaymentProfile : Profile
	{
		public PaymentProfile()
		{
			CreateMap<PaymentCreateDTO, Payment>()
				.ForPath(dest => dest.Amount, opt => opt.MapFrom(src => new Money(src.Amount, src.Currency)))
				.ForPath(dest => dest.PaymentMethod, opt => opt.MapFrom(src => Enum.Parse<PaymentMethod>(src.PaymentMethod)))
				.ForPath(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<PaymentStatus>(src.PaymentStatus)))
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

			CreateMap<PaymentTransaction, PaymentTransactionResponseDTO>()
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
				.ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency));

			CreateMap<RefundCreateDTO, Refund>()
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => new Money(src.Amount, "USD"))) // Default currency, should be resolved from context
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.ProcessedAt, opt => opt.Ignore())
				.ForMember(dest => dest.ExternalRefundId, opt => opt.Ignore());

			CreateMap<Refund, RefundResponseDTO>()
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
				.ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency));

			// Invoice mappings
			CreateMap<InvoiceCreateDTO, Invoice>()
				.ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => new Address(
					src.BillingStreet,
					src.BillingCity,
					src.BillingPostalCode)))
				.ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => new Money(0, src.Currency)))
				.ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => new Money(0, src.Currency)))
				.ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => new Money(0, src.Currency)))
				.ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => new Money(0, src.Currency)))
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.IssuedDate, opt => opt.Ignore())
				.ForMember(dest => dest.PaidDate, opt => opt.Ignore())
				.ForMember(dest => dest.LineItems, opt => opt.Ignore())
				.ForMember(dest => dest.Payments, opt => opt.Ignore())
				.ForMember(dest => dest.RowVersion, opt => opt.Ignore());

			CreateMap<InvoiceUpdateDTO, Invoice>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InvoiceId))
				.ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src =>
					!string.IsNullOrEmpty(src.BillingStreet) ? new Address(
						src.BillingStreet,
						src.BillingCity ?? string.Empty,
						src.BillingPostalCode ?? string.Empty) : null))
				.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<Invoice, InvoiceResponseDTO>()
				.ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal.Amount))
				.ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount.Amount))
				.ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount.Amount))
				.ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
				.ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
				.ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Recipient))
				.ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.BillingAddress))
				.ForMember(dest => dest.LineItems, opt => opt.MapFrom(src => src.LineItems))
				.ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
				.ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()))
				.ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.GetOutstandingAmount().Amount))
				.ForMember(dest => dest.DaysUntilDue, opt => opt.MapFrom(src => src.GetDaysUntilDue()))
				.ForMember(dest => dest.IsSuccess, opt => opt.MapFrom(src => true))
				.ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Invoice retrieved successfully"))
				.ForMember(dest => dest.Errors, opt => opt.MapFrom(src => new List<string>()));

			// InvoiceLineItem mappings
			CreateMap<InvoiceLineItemCreateDTO, InvoiceLineItem>()
				.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => new Money(src.UnitPrice, "USD"))) // Currency should be resolved from context
				.ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.InvoiceId, opt => opt.Ignore());

			CreateMap<InvoiceLineItemUpdateDTO, InvoiceLineItem>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LineItemId))
				.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src =>
					src.UnitPrice.HasValue ? new Money(src.UnitPrice.Value, "USD") : null))
				.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<InvoiceLineItem, InvoiceLineItemResponseDTO>()
				.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
				.ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice.Amount))
				.ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.UnitPrice.Currency));

		}
	}
}
