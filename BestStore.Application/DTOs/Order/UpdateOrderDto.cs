namespace BestStore.Application.DTOs.Order
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }
        public string PaymentStatus { get; set; } = "";
        public string OrderStatus { get; set; } = "";
    }
}
