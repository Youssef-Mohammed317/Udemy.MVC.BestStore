using AutoMapper;
using Azure;
using BestStore.Application.DTOs.Cart;
using BestStore.Application.Interfaces.Services;
using BestStore.Domain;
using BestStore.Domain.Entities;
using BestStore.Web.Helpers;
using BestStore.Web.Models.ViewModels.Cart;
using BestStore.Web.Models.ViewModels.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BestStore.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly PayPalSettings _payPalSettings;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public CheckoutController(IOptions<PayPalSettings> options, IOrderService orderService, IMapper mapper)
        {
            _payPalSettings = options.Value;
            this._orderService = orderService;
            this._mapper = mapper;
        }

        public IActionResult Index()
        {
            var cartViewJson = HttpContext.Session.GetString("CartViewModel");
            if (cartViewJson == null)
                return RedirectToAction("Index");

            var cartView = JsonSerializer.Deserialize<CartViewModel>(cartViewJson);

            if (cartView.CartSize == 0 || string.IsNullOrEmpty(cartView.CheckoutViewModel.DeliveryAddress) || string.IsNullOrEmpty(cartView.CheckoutViewModel.PaymentMethod))
            {
                return RedirectToAction("Index", "Home");
            }
            var vm = new CheckoutPayPalViewModel
            {
                ClientId = _payPalSettings.ClientId,
                DeliveryAddress = cartView.CheckoutViewModel.DeliveryAddress,
                TotalAmount = cartView.Total,
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {

            var cartViewJson = HttpContext.Session.GetString("CartViewModel");
            if (cartViewJson == null)
                return RedirectToAction("Index");

            var cartView = JsonSerializer.Deserialize<CartViewModel>(cartViewJson);

            if (cartView.CartSize == 0 || string.IsNullOrEmpty(cartView.CheckoutViewModel.DeliveryAddress) || string.IsNullOrEmpty(cartView.CheckoutViewModel.PaymentMethod))
            {
                return RedirectToAction("Index", "Home");
            }


            // create the request body
            JsonObject createOrderRequest = new JsonObject();
            createOrderRequest.Add("intent", "CAPTURE");

            JsonObject amount = new JsonObject();
            amount.Add("currency_code", "USD");
            amount.Add("value", cartView.Total);

            JsonObject purchaseUnit1 = new JsonObject();
            purchaseUnit1.Add("amount", amount);

            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchaseUnit1);

            createOrderRequest.Add("purchase_units", purchaseUnits);

            var accessToken = await GetPaypalAccessToken();

            string url = _payPalSettings.Url + "/v2/checkout/orders";


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonContent = new StringContent(JsonSerializer.Serialize(createOrderRequest), null, "application/json");

                requestMessage.Content = jsonContent;


                var httpResponse = await client.SendAsync(requestMessage);


                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        string paypalOrderId = jsonResponse["id"]?.ToString() ?? "";
                        return new JsonResult(new { Id = paypalOrderId });
                    }

                }

            }



            return new JsonResult(new { Id = "" });
        }


        [HttpPost]
        public async Task<IActionResult> CompleteOrder([FromBody] JsonObject data)
        {
            var orderId = data?["orderID"]?.ToString();
            var deliveryAddress = data?["deliveryAddress"]?.ToString();

            if (orderId == null || deliveryAddress == null)
            {
                return new JsonResult("error");
            }

            // get access token
            string accessToken = await GetPaypalAccessToken();


            string url = _payPalSettings.Url + "/v2/checkout/orders/" + orderId + "/capture";


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", null, "application/json");

                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
                        if (paypalOrderStatus == "COMPLETED")
                        {
                            // save the order in the database
                            await SaveOrderAsync(jsonResponse.ToString(), deliveryAddress);

                            return new JsonResult("success");
                        }
                    }
                }
            }


            return new JsonResult("error");

        }
        private async Task SaveOrderAsync(string paypalResponse, string deliveryAddress)
        {
            var cartViewJson = HttpContext.Session.GetString("CartViewModel");
            if (cartViewJson == null)
                return;

            var cartView = JsonSerializer.Deserialize<CartViewModel>(cartViewJson);

            if (cartView.CartSize == 0 || string.IsNullOrEmpty(cartView.CheckoutViewModel.DeliveryAddress) || string.IsNullOrEmpty(cartView.CheckoutViewModel.PaymentMethod))
            {
                return;
            }

            cartView.CheckoutViewModel.DeliveryAddress = deliveryAddress;
            cartView.CheckoutViewModel.PaymentDetails = paypalResponse;

            var cartDto = _mapper.Map<CartDto>(cartView);
            var result = await _orderService.CreateCartOrderAsync(cartDto, true);


            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Message;
            }
            else
            {
                // delete the shopping cart cookie
                Response.Cookies.Delete("shopping_cart");

                TempData["SuccessCartMessage"] = "Order created successfully";
            }

        }


        private async Task<string> GetPaypalAccessToken()
        {
            string accessToken = "";

            var url = _payPalSettings.Url + "/v1/oauth2/token";

            using (var client = new HttpClient())
            {
                string credentials64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_payPalSettings.ClientId}:{_payPalSettings.Secret}"));
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials64}");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                requestMessage.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");

                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        accessToken = jsonResponse["access_token"]?.ToString() ?? "";
                    }
                }

            }

            return accessToken;
        }
    }
}
