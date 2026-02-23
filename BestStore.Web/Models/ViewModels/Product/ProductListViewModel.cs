using BestStore.Domain.Result;
using BestStore.Web.Models.ViewModels.Category;

namespace BestStore.Web.Models.ViewModels.Product
{
    public class ProductListViewModel
    {
        public PaginatedResult<ProductViewModel> PaginatedProducts { get; set; } = new();
        public string Search { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string SortBy { get; set; } = nameof(ProductViewModel.Name);
        public bool Ascending { get; set; } = true;
        public List<CategorySelectPairViewModel> CategorySelectPairs { get; set; } = null!;
    }
}

