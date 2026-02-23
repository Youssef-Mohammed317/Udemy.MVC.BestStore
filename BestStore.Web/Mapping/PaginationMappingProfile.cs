using AutoMapper;
using BestStore.Application.DTOs.Product;
using BestStore.Domain.Result;
using BestStore.Web.Models.ViewModels.Product;

namespace BestStore.Web.Mapping
{
    public class PaginationMappingProfile : Profile
    {
        public PaginationMappingProfile()
        {
            CreateMap<PaginatedResult<ProductDto>, PaginatedResult<ProductViewModel>>();

        }
    }
}
