using BestStore.Domain.Entities;
using BestStore.Domain.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Interfaces.Repositories
{

    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Result<List<Category>>> GetAllCategoriesAsync();
    }
}
