using AutoMapper;
using Core.SharedKernel.Enums;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Profiles
{
	public class CraftmanProfileMap : Profile
	{
		public CraftmanProfileMap()
		{
			CreateMap<CraftmanCreateDTO, Craftman>()
				.ForPath(dest => dest.Email.Address, opt => opt.MapFrom(src => src.Email))
				//.ForMember(dest => dest.Profession, opt => opt.MapFrom(src => Enum.Parse<Profession>(src.Profession)))
				//.ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<VerificationStatus>(src.Status)))
				//.ForPath(dest => dest.HourlyRate.Amount, opt => opt.MapFrom(src => src.HourlyRate))
				//.ForPath(dest => dest.HourlyRate.Currency, opt => opt.MapFrom(src => src.Currency))
				.ForMember(dest => dest.Skills, opt => opt.Ignore());

			CreateMap<CraftsmanProfileUpdateDTO, Craftman>()
				.ForMember(dest => dest.Profession, opt => opt.MapFrom(src => Enum.Parse<Profession>(src.Profession)))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<VerificationStatus>(src.VerificationStatus)))
				.ForPath(dest => dest.Email.Address, opt => opt.MapFrom(src => src.EmailAddress))
				.ForPath(dest => dest.Phone.Number, opt => opt.MapFrom(src => src.PhoneNumber));

			CreateMap<Craftman, CraftmanResponseDTO>()
				.ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills.Select(s => s.Name).ToList()))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Address))
				.ForMember(dest => dest.Profession, opt => opt.MapFrom(src => src.Profession.ToString()))
				.ForMember(dest => dest.HourlyRate, opt => opt.MapFrom(src => src.HourlyRate!.Amount))
				.ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.HourlyRate!.Currency))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
		}
	}
}
