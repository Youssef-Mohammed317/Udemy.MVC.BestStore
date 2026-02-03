using BestStore.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BestStore.Application.DTOs.Cart
{
    public class CartDto
    {
        public List<OrderItemDto> CartItems { get; set; } = new();
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
        public CheckoutDto CheckoutDto { get; set; } = new();

    }
    public class CheckoutDto
    {
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
