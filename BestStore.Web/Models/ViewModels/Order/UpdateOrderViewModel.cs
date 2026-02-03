namespace BestStore.Web.Models.ViewModels.Order
{
    public class UpdateOrderViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string ClientId { get; set; } = "";
        public string ClientName { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
        public decimal ShippingFee { get; set; }
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
        public string PaymentDetails { get; set; } = ""; // to store paypal details
        public string OrderStatus { get; set; } = "";
    }
}
