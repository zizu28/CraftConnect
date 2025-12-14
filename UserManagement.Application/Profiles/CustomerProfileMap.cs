using AutoMapper;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects; // Ensure Value Objects are imported
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Profiles
{
	public class CustomerProfileMap : Profile
	{
		public CustomerProfileMap()
		{
			// --- CREATE ---
			CreateMap<CustomerCreateDTO, Customer>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => new Email(src.Email)));

			// --- UPDATE ---
			CreateMap<CustomerUpdateDTO, Customer>()
				.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
				.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
				.ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
				.ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => new Email(src.Email!)))
				.ForMember(dest => dest.Phone, opt => opt.MapFrom(src => new PhoneNumber("", src.Phone!)))
				.ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
					new Address(
						src.Street ?? "",
						src.City ?? "",
						src.PostalCode ?? ""
					)))
				.ForMember(dest => dest.PreferredPaymentMethod, opt => opt.MapFrom(src =>
					Enum.Parse<PaymentMethod>(src.PreferredPaymentMethod ?? "Cash", true)));

			// --- RESPONSE ---
			CreateMap<Customer, CustomerResponseDTO>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Address))
				.ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.Number))
				.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))

				// Flatten Address for display
				.ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address != null ? src.Address.Street : ""))
				.ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : ""))
				.ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Address != null ? src.Address.PostalCode : ""))
				.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address != null ? src.Address.ToString() : ""))

				.ForMember(dest => dest.PreferredPaymentMethod, opt => opt.MapFrom(src => src.PreferredPaymentMethod.ToString()));
		}
	}
}