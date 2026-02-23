using System;
using System.Collections.Generic;
using System.Text;
using BestStore.Domain.Entities;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Infrastructure.Contexts;

namespace BestStore.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
