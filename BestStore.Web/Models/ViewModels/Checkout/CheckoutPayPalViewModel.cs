namespace BestStore.Web.Models.ViewModels.Checkout
{
    public class CheckoutPayPalViewModel
    {
        public string ClientId { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
