using BestStore.Shared.Entities;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Infrastructure.Contexts;

namespace BestStore.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

    }
}
