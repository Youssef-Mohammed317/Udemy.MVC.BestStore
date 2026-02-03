
using BestStore.Application.DTOs.Product;
using BestStore.Shared.Entities;
using BestStore.Shared.Entities.Base;

namespace BestStore.Application.DTOs.Order
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductId { get; set; }
        public ProductDto Product { get; set; } = new();
    }
}
