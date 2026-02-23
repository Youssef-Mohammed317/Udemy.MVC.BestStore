using BestStore.Application.DTOs.Category;
using BestStore.Domain.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task<Result> DeleteCategoryAsync(int id);
        Task<Result<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<Result<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<Result<CategoryDto>> UpdateCategoryAsync(UpdateCategoryDto categoryDto);
    }
}
