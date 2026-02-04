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
        Task<Result<OrderDto>> GetOrderDetailsByIdAsync(int id);
        Task<Result<PaginatedResult<OrderDto>>> GetOrdersPaginatedAsync(string search = null, string sortBy = "CreatedAt", bool ascending = false, int pageNumber = 1, int pageSize = 10);
        Task<Result<OrderDto>> UpdateOrderPaymentStatusAsync(UpdateOrderDto orderDto);
    }
}
