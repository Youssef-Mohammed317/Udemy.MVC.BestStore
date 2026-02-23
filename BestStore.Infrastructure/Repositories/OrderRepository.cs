using BestStore.Domain.Entities;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BestStore.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> GetNumberOfOrderByUserId(string userId)
        {
            var count = await _dbSet.CountAsync(c => c.ClientId == userId);

            return count;
        }

    }
}
