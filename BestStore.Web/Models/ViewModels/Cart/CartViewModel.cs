using BestStore.Web.Models.ViewModels.Order;
using System.ComponentModel.DataAnnotations;

namespace BestStore.Web.Models.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<OrderItemViewModel> CartItems { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total => ShippingFee + SubTotal;
        public int CartSize
        {
            get
            {
                var size = 0;
                foreach (var item in CartItems)
                {
                    size += item.Quantity;
                }
                return size;
            }
        }
        public CheckoutViewModel CheckoutViewModel { get; set; }

    }
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "The Delivery Address is required.")]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
