using BestStore.Domain.Entities;
using BestStore.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
    }
}
