using BestStore.Domain.Result;

namespace BestStore.Web.Models.ViewModels.User
{
    public class UserListViewModel
    {
        public PaginatedResult<UserViewModel> PaginatedUsers { get; set; } = new();
        public string Search { get; set; } = string.Empty;
        public string SortBy { get; set; } = nameof(UserViewModel.FirstName);
        public bool Ascending { get; set; } = false;
    }

}
