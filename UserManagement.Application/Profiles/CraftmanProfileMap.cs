using AutoMapper;
using Core.SharedKernel.Domain;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
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
				.ForMember(dest => dest.Profession, opt => opt.MapFrom(src =>
					string.IsNullOrEmpty(src.Profession)
						? Profession.None
						: Enum.Parse<Profession>(src.Profession, true)))
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
					new Email(src.EmailAddress)))
				.ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
					new PhoneNumber("", src.PhoneNumber)))
				.ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
					src.Skills != null ?
					src.Skills.Select(s => new Skill(s.Name, s.YearsOfExperience)).ToList() : 
					new List<Skill>()))
				.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
				.ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
				.ForMember(dest => dest.Portfolio, opt => opt.MapFrom(src => 
					src.Portfolio != null ?
					src.Portfolio.Select(p => new Project(p.Title, p.Description, p.ImageUrl)) :
					new List<Project>()))
				.ForMember(dest => dest.WorkExperience, opt => opt.MapFrom(src => 
					src.WorkExperience != null ?
					src.WorkExperience.Select(w => new WorkEntry(w.Company, w.Position, w.Responsibilities, w.StartDate, w.EndDate)) :
					new List<WorkEntry>()));

			CreateMap<Craftman, CraftmanResponseDTO>()
				.ForMember(dest => dest.CraftmanId, opt => opt.MapFrom(src => src.Id))
				.ForPath(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills!.Select(s => new SkillsDTO(s.Name, s.YearsOfExperience)).ToList()))
				.ForPath(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email.Address))
				.ForMember(dest => dest.Profession, opt => opt.MapFrom(src => src.Profession.ToString()))
				.ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl));
		}
	}
}
