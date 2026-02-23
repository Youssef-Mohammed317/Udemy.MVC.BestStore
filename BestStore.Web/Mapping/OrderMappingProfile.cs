using AutoMapper;
using BestStore.Application.DTOs.Order;
using BestStore.Domain.Result;
using BestStore.Web.Models.ViewModels.Order;

namespace BestStore.Web.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {

            CreateMap<OrderItemViewModel, OrderItemDto>().ReverseMap();
            CreateMap<OrderDto, OrderViewModel>().ReverseMap();
            CreateMap<OrderDetailsDto, OrderDetailsViewModel>().ReverseMap();
            CreateMap<UpdateOrderViewModel, UpdateOrderDto>().ReverseMap();
            CreateMap<PaginatedResult<OrderDto>, PaginatedResult<OrderViewModel>>().ReverseMap();
        }
    }

}
