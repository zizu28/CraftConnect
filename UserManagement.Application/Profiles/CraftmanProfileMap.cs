using AutoMapper;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
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
				.ForPath(dest => dest.Phone.Number, opt => opt.MapFrom(src => src.PhoneNumber))
				.ForPath(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills
					.Select(s => new Skill(s.Name, s.YearsOfExperience))))
				.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
				.ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
				.ForMember(dest => dest.Portfolio, opt => opt.MapFrom(src => src.Portfolio))
				.ForMember(dest => dest.WorkExperience, opt => opt.MapFrom(src => src.WorkExperience));

			CreateMap<Craftman, CraftmanResponseDTO>()
				.ForPath(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills!.Select(s => new SkillsDTO(s.Name, s.YearsOfExperience)).ToList()))
				.ForPath(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email.Address))
				.ForMember(dest => dest.Profession, opt => opt.MapFrom(src => src.Profession.ToString()));
		}
	}
}
