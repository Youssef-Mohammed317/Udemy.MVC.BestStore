using BestStore.Application.DTOs.Product;
using BestStore.Domain.Result;

namespace BestStore.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Result<List<ProductDto>>> GetAllProductsAsync();
        Task<Result<ProductDetailsDto>> GetProductDetailsByIdAsync(int id);

        Task<Result<ProductDto>> CreateProductAsync(CreateProductDto productDto, string rootPath);
        Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto productDto, string rootPath);
        Task<Result<ProductDto>> GetProductByIdAsync(int id);
        Task<Result> DeleteProductAsync(int id, string rootPath);

        Task<Result<PaginatedResult<ProductDto>>> GetProductsPaginatedAsync(
        string search = null,
        int? category = null,
        string sortBy = nameof(ProductDto.Name),
        bool ascending = true,
        int pageNumber = 1,
        int pageSize = 10);
    }
}
