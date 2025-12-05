using AutoMapper;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Profiles
{
	public class CustomerProfileMap : Profile
	{
		public CustomerProfileMap()
		{
			CreateMap<CustomerCreateDTO, Customer>()
				.ForPath(dest => dest.Email.Address, opt => opt.MapFrom(src => src.Email));
				//.ForPath(dest => dest.Address!.Street, opt => opt.MapFrom(src => src.Street))
				//.ForPath(dest => dest.Address!.City, opt => opt.MapFrom(src => src.City))
				//.ForPath(dest => dest.Address!.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
				//.ForPath(dest => dest.Address!.Location!.Latitude, opt => opt.MapFrom(src => src.Latitude))
				//.ForPath(dest => dest.Address!.Location!.Longitude, opt => opt.MapFrom(src => src.Longitude))
				//.ForMember(dest => dest.PreferredPaymentMethod, opt => opt.MapFrom(src => Enum.Parse<PaymentMethod>(src.PreferredPaymentMethod)));

			CreateMap<CustomerUpdateDTO, Customer>()
				.ForPath(dest => dest.Email.Address, opt => opt.MapFrom(src => src.Email))
				.ForPath(dest => dest.Address!.Street, opt => opt.MapFrom(src => src.Street))
				.ForPath(dest => dest.Address!.City, opt => opt.MapFrom(src => src.City))
				.ForPath(dest => dest.Address!.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
				.ForMember(dest => dest.PreferredPaymentMethod, opt => opt.MapFrom(src => Enum.Parse<PaymentMethod>(src.PreferredPaymentMethod)));

			CreateMap<Customer, CustomerResponseDTO>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Address))
				.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.PreferredPaymentMethod, opt => opt.MapFrom(src => src.PreferredPaymentMethod.ToString()))
				.ForMember(dest => dest.Address, opt => opt.MapFrom(src 
				=> $"{src.Address!.Street}, {src.Address!.Location}. {src.Address!.PostalCode}".ToString()));
		}
	}
}
