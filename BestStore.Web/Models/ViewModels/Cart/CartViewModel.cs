using BestStore.Web.Models.ViewModels.Order;

namespace BestStore.Web.Models.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<OrderItemViewModel> CartItems { get; set; } = new();
        public decimal ShippingFee { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total => ShippingFee + SubTotal;
        public int CartSize
        {
            get
            {
                var size = 0;
                foreach (var item in this.CartItems)
                {
                    size += item.Quantity;
                }
                return size;
            }
        }
        public CheckoutViewModel CheckoutViewModel { get; set; } = new();

    }
}
