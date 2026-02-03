using BestStore.Application.DTOs.Cart;
using BestStore.Shared.Result;

namespace BestStore.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Result> CreateOrderAsync(CartDto cartDto);
    }
}
