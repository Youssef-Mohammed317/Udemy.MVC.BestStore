using BestStore.Shared.Entities;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Infrastructure.Contexts;

namespace BestStore.Infrastructure.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
