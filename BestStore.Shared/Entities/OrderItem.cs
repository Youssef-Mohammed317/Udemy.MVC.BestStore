using BestStore.Shared.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestStore.Shared.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Product Product { get; set; } = new Product();
    }
}
