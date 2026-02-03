using BestStore.Shared.Entities.Base;


namespace BestStore.Shared.Entities
{
    public class Order : BaseEntity
    {
        public string ClientId { get; set; } = "";

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal ShippingFee { get; set; }

        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
        public string PaymentDetails { get; set; } = ""; // to store paypal details
        public string OrderStatus { get; set; } = "";
    }
}
