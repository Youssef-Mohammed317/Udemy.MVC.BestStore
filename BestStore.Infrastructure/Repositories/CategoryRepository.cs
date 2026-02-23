using BestStore.Domain.Entities;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Infrastructure.Contexts;
using BestStore.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace BestStore.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<Result<List<Category>>> GetAllCategoriesAsync()
        {
            var data = await _dbSet.ToListAsync();
            return Result<List<Category>>.Success(data);
        }
    }
}
