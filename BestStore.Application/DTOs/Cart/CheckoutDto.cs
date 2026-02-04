namespace BestStore.Application.DTOs.Cart
{
    public class CheckoutDto
    {
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentDetails { get; set; } = "";
    }
}
