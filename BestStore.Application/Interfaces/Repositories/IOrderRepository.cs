using BestStore.Shared.Entities;

namespace BestStore.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<int> GetNumberOfOrderByUserId(string userId);
    }
}
