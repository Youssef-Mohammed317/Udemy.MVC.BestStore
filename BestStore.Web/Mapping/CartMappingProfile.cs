using AutoMapper;
using BestStore.Application.DTOs.Cart;
using BestStore.Domain.Entities;
using BestStore.Web.Models.ViewModels.Cart;

namespace BestStore.Web.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {


            CreateMap<CartViewModel, CartDto>()
                .ForMember(dest => dest.CheckoutDto, opt => opt.MapFrom(src => src.CheckoutViewModel));
            CreateMap<CartDto, CartViewModel>()
                .ForMember(dest => dest.CheckoutViewModel, opt => opt.MapFrom(src => src.CheckoutDto));
                


            // Checkout
            CreateMap<CheckoutDto, CheckoutViewModel>().ReverseMap();
            CreateMap<CheckoutViewModel, CheckoutDto>().ReverseMap();

        }
    }

}
