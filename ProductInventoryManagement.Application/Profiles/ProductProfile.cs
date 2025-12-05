using AutoMapper;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.ValueObjects;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Application.Profiles
{
	public class ProductProfile : Profile
	{
		public ProductProfile()
		{
			CreateMap<ProductCreateDTO, Product>()
				.ForCtorParam("name", opt => opt.MapFrom(src => src.Name))
				.ForCtorParam("description", opt => opt.MapFrom(src => src.Description))
				.ForCtorParam("price", opt => opt.MapFrom(src => src.Price))
				.ForCtorParam("categoryId", opt => opt.MapFrom(src => src.CategoryId));

			CreateMap<ProductUpdateDTO, Product>()
				.ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
				.ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
				.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

			CreateMap<Product, ProductResponseDTO>()
				.ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Inventory.Quantity))
				.ForMember(dest => dest.CraftmanId, opt => opt.MapFrom(src => src.CraftmanId))
				.ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

			CreateMap<Image, ImageResponseDTO>();
		}
	}
}