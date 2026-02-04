using BestStore.Shared.Result;
using BestStore.Web.Models.ViewModels.Category;
using BestStore.Web.Models.ViewModels.Product;

namespace BestStore.Web.Models.ViewModels.Order
{
    public class OrderListViewModel
    {
        public PaginatedResult<OrderViewModel> PaginatedOrders { get; set; } = new();
        public string Search { get; set; } = string.Empty;
        public string SortBy { get; set; } = nameof(OrderViewModel.CreatedAt);
        public bool Ascending { get; set; } = false;
    }
}
