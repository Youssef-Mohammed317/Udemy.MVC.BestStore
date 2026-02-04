namespace BestStore.Web.Models.ViewModels.Order
{
    public class UpdateOrderViewModel
    {
        public int Id { get; set; }

        public string PaymentStatus { get; set; } = "";

        public string OrderStatus { get; set; } = "";
    }
}
