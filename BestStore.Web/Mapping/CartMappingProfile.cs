using AutoMapper;
using BestStore.Application.DTOs.Cart;
using BestStore.Application.DTOs.Order;
using BestStore.Web.Models.ViewModels.Cart;
using BestStore.Web.Models.ViewModels.Order;

namespace BestStore.Web.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {


            CreateMap<CartViewModel, CartDto>()
           .ForMember(dest => dest.CartItems,
               opt => opt.MapFrom(src => src.CartItems))
           .ForMember(dest => dest.CheckoutDto,
               opt => opt.MapFrom(src => src.CheckoutViewModel))
           .ForMember(dest => dest.Total, opt => opt.Ignore())
           .ForMember(dest => dest.CartSize, opt => opt.Ignore());

            CreateMap<CartDto, CartViewModel>()
                .ForMember(dest => dest.CartItems,
                    opt => opt.MapFrom(src => src.CartItems))
                .ForMember(dest => dest.CheckoutViewModel,
                    opt => opt.MapFrom(src => src.CheckoutDto))
                .ForMember(dest => dest.Total, opt => opt.Ignore())
                .ForMember(dest => dest.CartSize, opt => opt.Ignore());

            // Checkout
            CreateMap<CheckoutDto, CheckoutViewModel>()
                .ReverseMap();

            // Order Item
            CreateMap<OrderItemDto, OrderItemViewModel>()
                .ReverseMap();


        }
    }

}
