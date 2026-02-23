using AutoMapper;
using BestStore.Application.DTOs.Category;
using BestStore.Domain.Entities;
using BestStore.Web.Models.ViewModels.Category;

namespace BestStore.Web.Mapping
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<CategoryDto, CategorySelectPairViewModel>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();



            CreateMap<CategoryDto, CategoryViewModel>().ReverseMap();
            CreateMap<CategoryDto, UpdateCategoryViewModel>().ReverseMap();

            CreateMap<CreateCategoryDto, CreateCategoryViewModel>().ReverseMap();
            CreateMap<UpdateCategoryDto, UpdateCategoryViewModel>().ReverseMap();

        }
    }

}
