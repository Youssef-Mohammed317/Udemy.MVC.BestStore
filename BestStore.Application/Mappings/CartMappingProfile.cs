using AutoMapper;
using BestStore.Application.DTOs.Order;
using BestStore.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Mappings
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        }
    }
}
