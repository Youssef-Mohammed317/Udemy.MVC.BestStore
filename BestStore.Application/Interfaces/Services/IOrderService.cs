using BestStore.Application.DTOs.Cart;
using BestStore.Application.DTOs.Order;
using BestStore.Shared.Result;

namespace BestStore.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Result> CreateCartOrderAsync(CartDto cartDto);
        Task<Result<List<OrderDto>>> GetAllOrdersAsync();
        Task<Result<OrderDto>> GetOrderByIdAsync(int id);
        Task<Result<OrderDto>> UpdateOrderAsync(UpdateOrderDto orderDto);
    }
}
