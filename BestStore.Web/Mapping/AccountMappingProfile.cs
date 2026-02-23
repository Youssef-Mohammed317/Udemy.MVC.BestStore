using AutoMapper;
using BestStore.Application.DTOs.Account;
using BestStore.Domain.Entities;
using BestStore.Web.Models.ViewModels.Account;

namespace BestStore.Web.Mapping
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {


            CreateMap<RegisterViewModel, RegisterDto>();
            CreateMap<LoginViewModel, LoginDto>();
            CreateMap<ResetPasswordViewModel, ResetPasswordDto>();
            CreateMap<ChangePasswordViewModel, ChangePasswordDto>();

            CreateMap<SendConfirmEmailViewModel, SendConfirmEmailDto>().ReverseMap();
            CreateMap<EmailConfirmaitonDto, EmailConfirmaitonViewModel>().ReverseMap();

            CreateMap<UpdateProfileDto, UpdateProfileViewModel>().ReverseMap();

        }
    }
}
