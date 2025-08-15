using AutoMapper;
using BookingManagement.Application.DTOs.JobDetailsDTOs;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Profiles
{
	public class JobDetailsProfile : Profile
	{
		public JobDetailsProfile()
		{
			CreateMap<JobDetails, JobDetailsResponseDTO>();
		}
	}
}
