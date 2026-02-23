using AutoMapper;
using BestStore.Application.DTOs.Category;
using BestStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();
        }
    }
}
