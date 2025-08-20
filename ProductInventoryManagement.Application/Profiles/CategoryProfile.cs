using AutoMapper;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Application.Profiles
{
	public class CategoryProfile : Profile
	{
		public CategoryProfile()
		{
			CreateMap<CategoryCreateDTO, Category>()
				.ForCtorParam("name", opt => opt.MapFrom(src => src.Name))
				.ForCtorParam("description", opt => opt.MapFrom(src => src.Description))
				.ForCtorParam("parentCategoryId", opt => opt.MapFrom(src => src.ParentCategoryId));

			CreateMap<CategoryUpdateDTO, Category>()
				.ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
				.ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
				.ForMember(dest => dest.ParentCategoryId, opt => opt.Condition(src => src.ParentCategoryId.HasValue));

			CreateMap<Category, CategoryResponseDTO>()
				.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id));
		}
	}
}