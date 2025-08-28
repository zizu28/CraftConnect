using AutoMapper;
using BookingManagement.Application.DTOs.JobDetailsDTOs;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Profiles
{
	public class JobDetailsProfile : Profile
	{
		public JobDetailsProfile()
		{
			CreateMap<JobDetailsCreateDTO, JobDetails>()
				.ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

			CreateMap<JobDetailsUpdateDTO, JobDetails>()
				.ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

			CreateMap<JobDetails, JobDetailsResponseDTO>()
				.ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
		}
	}
}
