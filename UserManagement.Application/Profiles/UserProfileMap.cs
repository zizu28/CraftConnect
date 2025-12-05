using AutoMapper;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Profiles
{
	public class UserProfileMap : Profile
	{
		public UserProfileMap()
		{
			CreateMap<UserCreateDTO, User>()
				.ForPath(dest => dest.Email.Address, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.AgreeToTerms, opt => opt.MapFrom(src => src.AgreeToTerms))
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role))); ;

			CreateMap<UserUpdateDTO, User>()
				.ForPath(dest => dest.Email.Address, opt => opt.MapFrom(src => src.Email))
				.ForPath(dest => dest.Phone.Number, opt => opt.MapFrom(src => src.PhoneNumber))
				.ForPath(dest => dest.Phone.CountryCode, opt => opt.MapFrom(src => src.PhoneCountryCode))
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));

			CreateMap<User, UserResponseDTO>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Address))
				.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone.Number))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
		}
	}
}
