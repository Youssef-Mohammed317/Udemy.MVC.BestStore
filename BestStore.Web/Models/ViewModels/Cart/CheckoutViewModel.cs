using System.ComponentModel.DataAnnotations;

namespace BestStore.Web.Models.ViewModels.Cart
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "The Delivery Address is required.")]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";

        public string PaymentDetails { get; set; } = "";
    }
}
