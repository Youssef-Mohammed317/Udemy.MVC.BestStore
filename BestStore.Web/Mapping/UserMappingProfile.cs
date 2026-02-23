using BestStore.Application.DTOs.User;
using BestStore.Domain.Result;
using BestStore.Web.Models.ViewModels.User;

namespace BestStore.Web.Mapping
{
    public class UserMappingProfile : AutoMapper.Profile
    {
        public UserMappingProfile()
        {

            CreateMap<UserDto, UserViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt.ToString("yyyy-MM-dd")));

            CreateMap<UserDto, UserDetailsViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt.ToString("yyyy-MM-dd")));

            CreateMap<PaginatedResult<UserDto>, PaginatedResult<UserViewModel>>();

            CreateMap<RolePairDto, RolePairViewModel>();

            CreateMap<UserDto, ManageRolesViewModel>();


        }
    }
}
