using BestStore.Application.DTOs.Cart;
using BestStore.Application.DTOs.Order;
using BestStore.Shared.Result;

namespace BestStore.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Result> CreateCartOrderAsync(CartDto cartDto, bool isPaypalAccepted = false);
        Task<Result<OrderDetailsDto>> GetOrderByIdAsync(int id);
        Task<Result<OrderDetailsDto>> GetOrderDetailsByIdAsync(int id);
        Task<Result<PaginatedResult<OrderDto>>> GetOrdersPaginatedAsync(string? search = null, string sortBy = "CreatedAt", bool ascending = false, int pageNumber = 1, int pageSize = 10, string? userId = null, bool isAdmin = false);
        Task<Result<OrderDto>> UpdateOrderPaymentStatusAsync(UpdateOrderDto orderDto);
    }
}
