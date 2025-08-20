using AutoMapper;
using BookingManagement.Application.DTOs.BookingLineItemDTOs;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Profiles
{
	public class BookingLineItemProfile : Profile
	{
		public BookingLineItemProfile()
		{
			CreateMap<BookingLineItemCreateDTO, BookingLineItem>()
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
				.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

			CreateMap<BookingLineItem, BookingLineItemResponseDTO>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
				.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
		}
	}
}
