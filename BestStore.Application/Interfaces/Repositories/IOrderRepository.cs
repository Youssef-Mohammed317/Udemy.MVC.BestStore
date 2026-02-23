using BestStore.Domain.Entities;

namespace BestStore.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<int> GetNumberOfOrderByUserId(string userId);
    }
}
