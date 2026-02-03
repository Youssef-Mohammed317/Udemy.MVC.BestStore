using AutoMapper;
using BestStore.Application.Interfaces.Services;
using BestStore.Shared.Entities;
using BestStore.Web.Models.ViewModels.Order;
using BestStore.Web.Models.ViewModels.Product;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace BestStore.Web.Helpers;

public static class CartHelper
{
    public static Dictionary<int, int> GetCartDictionary(HttpRequest request, HttpResponse response)
    {
        string cookieValue = request.Cookies["shopping_cart"] ?? "";
        try
        {
            var cart = Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
            Console.WriteLine("[CartHelper] cart=" + cookieValue + " -> " + cart);
            var dic = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            if (dic != null)
            {
                return dic;
            }


        }
        catch (Exception)
        {
            Console.WriteLine("Error");
        }
        if (cookieValue.Length > 0)
        {
            response.Cookies.Delete("shopping_cart");
        }
        return new();
    }
    public static int GetCartSize(HttpRequest request, HttpResponse response)
    {
        int cartSize = 0;

        var cartDictionary = GetCartDictionary(request, response);
        foreach (var keyValuePair in cartDictionary)
        {
            cartSize += keyValuePair.Value;
        }

        return cartSize;
    }

    public static async Task<List<OrderItemViewModel>> GetCartItemsAsync(HttpRequest request,
        HttpResponse response, 
        IProductService productService,
        IMapper mapper)
    {
        var cartItems = new List<OrderItemViewModel>();
        var cartDictionary = GetCartDictionary(request, response);
        foreach (var pair in cartDictionary)
        {
            int productId = pair.Key;
            int quantity = pair.Value;
            var result = await productService.GetProductByIdAsync(productId);
            if (result.IsFailure) continue;

            var item = new OrderItemViewModel
            {
                Quantity = quantity,
                UnitPrice = result.Value.Price,
                Product = mapper.Map<ProductViewModel>(result.Value),
            };

            cartItems.Add(item);
        }


        return cartItems;
    }
    public static decimal GetSubtotal(List<OrderItemViewModel> cartItems)
    {
        decimal subtotal = 0;

        foreach (var item in cartItems)
        {
            subtotal += item.Quantity * item.UnitPrice;
        }

        return subtotal;
    }
}
