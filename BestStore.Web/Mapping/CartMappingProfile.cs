using AutoMapper;
using BestStore.Application.DTOs.Cart;
using BestStore.Shared.Entities;
using BestStore.Web.Models.ViewModels.Cart;

namespace BestStore.Web.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {


            CreateMap<CartViewModel, CartDto>();
            CreateMap<CartDto, CartViewModel>();


            // Checkout
            CreateMap<CheckoutDto, CheckoutViewModel>()
                .ReverseMap();

        }
    }

}
