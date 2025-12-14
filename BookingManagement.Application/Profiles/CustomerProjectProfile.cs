using AutoMapper;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.ValueObjects;

namespace BookingManagement.Application.Profiles
{
	public class CustomerProjectProfile : Profile
	{
		public CustomerProjectProfile()
		{
			CreateMap<CustomerProject, CustomerProjectResponseDTO>()
				.ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
				.ForMember(dest => dest.Budget, opt => opt.MapFrom(src =>
					new MoneyDTO { Amount = src.Budget.Amount, Currency = src.Budget.Currency }))
				.ForMember(dest => dest.Timeline, opt => opt.MapFrom(src =>
					new DateTimeRangeDTO { Start = src.Timeline.Start, End = src.Timeline.End }))
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Timeline.Start))
				.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Timeline.End))
				.ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
					src.Skills.Select(s => new SkillDTO { Name = s.Name, YearsOfExperience = s.YearsOfExperience })));

			CreateMap<CreateCustomerProjectDTO, CustomerProject>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.Budget, opt => opt.MapFrom(src =>
					new Money(src.Budget.Amount, src.Budget.Currency)))
				.ForMember(dest => dest.Timeline, opt => opt.MapFrom(src =>
					new DateTimeRange(src.Timeline.Start, src.Timeline.End)))
				.ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
					src.RequiredSkills.Select(s => new Skill(s.Name, s.YearsOfExperience)).ToList()));


			CreateMap<UpdateCustomerProjectDTO, CustomerProject>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CustomerId, opt => opt.Ignore())
				.ForMember(dest => dest.Budget, opt => opt.MapFrom(src =>
					new Money(src.Budget.Amount, src.Budget.Currency)))
				.ForMember(dest => dest.Timeline, opt => opt.MapFrom(src =>
					new DateTimeRange(src.Timeline.Start, src.Timeline.End)))
				.ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
					src.RequiredSkills.Select(s => new Skill(s.Name, s.YearsOfExperience)).ToList()));
		}
	}
}