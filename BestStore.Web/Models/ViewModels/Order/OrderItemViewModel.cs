
using BestStore.Web.Models.ViewModels.Product;

namespace BestStore.Web.Models.ViewModels.Order
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ProductViewModel Product { get; set; } = new();
    }
}
