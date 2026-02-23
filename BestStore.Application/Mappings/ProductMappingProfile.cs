using AutoMapper;
using BestStore.Application.DTOs.Product;
using BestStore.Domain.Entities;
using BestStore.Domain.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ReverseMap()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                ;

            CreateMap<CreateProductDto, Product>();


            CreateMap<UpdateProductDto, Product>();

            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));


            CreateMap<PaginatedResult<Product>, PaginatedResult<ProductDto>>();

        }
    }
}
