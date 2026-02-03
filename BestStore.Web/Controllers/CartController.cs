using AutoMapper;
using BestStore.Application.DTOs.Cart;
using BestStore.Application.DTOs.Order;
using BestStore.Application.Interfaces.Services;
using BestStore.Shared.Result;
using BestStore.Web.Helpers;
using BestStore.Web.Models.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BestStore.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly decimal _shippingFee;
        private readonly IMapper _mapper; private readonly string _errorMessageKey = "ErrorMessage";

        public CartController(IOrderService orderService, IConfiguration configuration,
            IProductService productService, IMapper mapper)
        {
            this._orderService = orderService;
            this._productService = productService;

            this._shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
            this._mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var cartItems = await CartHelper.GetCartItemsAsync(Request, Response, _productService, _mapper);
            decimal subtotal = CartHelper.GetSubtotal(cartItems);
            var cart = new CartViewModel
            {
                CartItems = cartItems,
                SubTotal = subtotal,
                ShippingFee = _shippingFee,
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(CartViewModel cartView)
        {
            var cartItems = await CartHelper.GetCartItemsAsync(
                   Request, Response, _productService, _mapper
               );
            if (!ModelState.IsValid)
            {

                cartView.CartItems = cartItems;
                cartView.SubTotal = CartHelper.GetSubtotal(cartItems);
                cartView.ShippingFee = _shippingFee;

                return View(cartView);
            }



            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty";
                return RedirectToAction("Index");
            }
            cartView.CartItems = cartItems;
            cartView.SubTotal = CartHelper.GetSubtotal(cartItems);
            cartView.ShippingFee = _shippingFee;

            var cartJson = JsonSerializer.Serialize(cartView);
            HttpContext.Session.SetString(
                "CartViewModel",
                cartJson
            );

            return RedirectToAction("Confirm");
        }
        [Authorize]
        public IActionResult Confirm()
        {
            var cartViewJson = HttpContext.Session.GetString("CartViewModel");
            if (cartViewJson == null)
                return RedirectToAction("Index");

            var cartView = JsonSerializer.Deserialize<CartViewModel>(cartViewJson);

            if (cartView.CartSize == 0 || string.IsNullOrEmpty(cartView.CheckoutViewModel.DeliveryAddress) || string.IsNullOrEmpty(cartView.CheckoutViewModel.PaymentMethod))
            {
                return RedirectToAction("Index", "Home");
            }


            return View(cartView);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPOST()
        {
            var cartViewJson = HttpContext.Session.GetString("CartViewModel");

            if (cartViewJson == null)
                return RedirectToAction("Index");

            CartViewModel cartView = JsonSerializer.Deserialize<CartViewModel>(cartViewJson);

            if (cartView.CartSize == 0 || string.IsNullOrEmpty(cartView.CheckoutViewModel.DeliveryAddress) || string.IsNullOrEmpty(cartView.CheckoutViewModel.PaymentMethod))
            {
                return RedirectToAction("Index", "Home");
            }
            var cartDto = _mapper.Map<CartDto>(cartView);
            var result = await _orderService.CreateCartOrderAsync(cartDto);


            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
            }
            else
            {


                // delete the shopping cart cookie
                Response.Cookies.Delete("shopping_cart");

                TempData["SuccessCartMessage"] = "Order created successfully";
            }

            return RedirectToAction("Confirm");
        }
    }
}
