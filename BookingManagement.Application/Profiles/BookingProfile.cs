using AutoMapper;
using BookingManagement.Application.DTOs;
using BookingManagement.Application.DTOs.BookingDTOs;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace BookingManagement.Application.Profiles
{
	public class BookingProfile : Profile
	{
		public BookingProfile()
		{
			CreateMap<Address, AddressDTO>();
			CreateMap<AddressDTO, Address>();

			CreateMap<BookingCreateDTO, Booking>()
				.ForPath(dest => dest.ServiceAddress.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
				.ForPath(dest => dest.ServiceAddress.City, opt => opt.MapFrom(src => src.City))
				.ForPath(dest => dest.ServiceAddress.Street, opt => opt.MapFrom(src => src.Street))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<BookingStatus>(src.Status)))
				.ForPath(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
				.ForPath(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));

			CreateMap<BookingUpdateDTO, Booking>()
				.ForPath(dest => dest.ServiceAddress.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
				.ForPath(dest => dest.ServiceAddress.City, opt => opt.MapFrom(src => src.City))
				.ForPath(dest => dest.ServiceAddress.Street, opt => opt.MapFrom(src => src.Street))
				.ForPath(dest => dest.Details.BookingId, opt => opt.MapFrom(src => src.BookingId))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<BookingStatus>(src.Status)))
				.ForPath(dest => dest.Details.Description, opt => opt.MapFrom(src => src.NewDescription));

			CreateMap<Booking, BookingResponseDTO>()
				.ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.CraftspersonId, opt => opt.MapFrom(src => src.CraftmanId))
				.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
				.ForPath(dest => dest.ServiceAddress.PostalCode, opt => opt.MapFrom(src => src.ServiceAddress.PostalCode))
				.ForPath(dest => dest.ServiceAddress.City, opt => opt.MapFrom(src => src.ServiceAddress.City))
				.ForPath(dest => dest.ServiceAddress.Street, opt => opt.MapFrom(src => src.ServiceAddress.Street))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
				.ForPath(dest => dest.Details.Description, opt => opt.MapFrom(src => src.Details.Description))
				.ForMember(dest => dest.LineItems, opt => opt.MapFrom(src => src.LineItems))
				.ForPath(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.CalculateTotalPrice()));
		}
	}
}
