using AutoMapper;
using BestStore.Application.DTOs.Category;
using BestStore.Application.DTOs.Product;
using BestStore.Domain.Entities;
using BestStore.Web.Models.ViewModels.Category;
using BestStore.Web.Models.ViewModels.Product;

namespace BestStore.Web.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductDto, ProductViewModel>().ReverseMap();

            CreateMap<CreateProductViewModel, CreateProductDto>();

            CreateMap<UpdateProductViewModel, UpdateProductDto>();

            CreateMap<ProductDto, UpdateProductViewModel>();

            CreateMap<ProductDetailsDto, ProductDetailsViewModel>();

            CreateMap<CategoryDto, CategorySelectPairViewModel>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id));

        }

    }
}
