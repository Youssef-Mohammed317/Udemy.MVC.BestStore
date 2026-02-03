
using BestStore.Application.DTOs.Product;

namespace BestStore.Application.DTOs.Order
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ProductDto Product { get; set; } = new();
    }
}
