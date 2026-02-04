using BestStore.Application.DTOs.User;

namespace BestStore.Application.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string ClientId { get; set; } = "";
        public UserDto Client { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal ShippingFee { get; set; }
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
        public string PaymentDetails { get; set; } = ""; // to store paypal details
        public string OrderStatus { get; set; } = "";
    }

}
