using AutoMapper;
using BestStore.Application.DTOs.Order;
using BestStore.Shared.Entities;

namespace BestStore.Application.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, OrderDetailsDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>()
                .ReverseMap()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                ;
            CreateMap<UpdateOrderDto, Order>().ReverseMap();
        }
    }
}
