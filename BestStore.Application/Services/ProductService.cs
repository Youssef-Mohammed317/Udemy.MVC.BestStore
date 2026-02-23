using AutoMapper;
using BestStore.Application.DTOs.Product;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Application.Interfaces.Services;
using BestStore.Application.Interfaces.Utility;
using BestStore.Domain.Entities;
using BestStore.Domain.Result;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BestStore.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IImageStorageService imageStorageService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._imageStorageService = imageStorageService;
        }
        public async Task<Result<List<ProductDto>>> GetAllProductsAsync()
        {
            var result = await _unitOfWork.ProductRepository.GetAllAsync(include: x => x.Include(x => x.Category));

            var productDtos = _mapper.Map<List<ProductDto>>(result.Value);

            return Result<List<ProductDto>>.Success(productDtos);
        }
        public async Task<Result<ProductDetailsDto>> GetProductDetailsByIdAsync(int id)
        {
            var productResult = await _unitOfWork.ProductRepository.GetByIdAsync(id, include: x => x.Include(x => x.Category));


            if (productResult.IsFailure)
            {
                return Result<ProductDetailsDto>.Failure(productResult.Error);
            }
            var product = productResult.Value;

            var relatedProductsResult = await _unitOfWork.ProductRepository.GetPaginatedAsync(
                filter: p => p.Id != id && p.CategoryId == product.CategoryId && p.Brand == product.Brand,
                include: x => x.Include(x => x.Category),
                pageNumber: 1,
                pageSize: 4
                );

            var relatedProductsDtos = _mapper.Map<PaginatedResult<ProductDto>>(relatedProductsResult.Value);

            var productDetailsDto = _mapper.Map<ProductDetailsDto>(product);

            productDetailsDto.RelatedProducts = relatedProductsDtos.Items;

            return Result<ProductDetailsDto>.Success(productDetailsDto);
        }

        public async Task<Result<ProductDto>> CreateProductAsync(CreateProductDto productDto, string rootPath)
        {
            var product = _mapper.Map<Product>(productDto);

            if (productDto.ImageFile == null)
            {
                return Result<ProductDto>.Failure(Error.Failure("Product.ImageMissing", "Product image is required."));
            }
            var uploadImageResult = await _imageStorageService.SaveImageAsync(productDto.ImageFile, rootPath, "products");
            if (uploadImageResult.IsFailure)
            {
                return Result<ProductDto>.Failure(Error.Failure("Product.ImageUploadFailed", "Failed to upload product image."));
            }

            product.ImageUrl = uploadImageResult.Value;

            await _unitOfWork.ProductRepository.AddAsync(product);
            var saveResult = await _unitOfWork.SaveChangesAsync();

            if (saveResult.IsSuccess)
            {
                var createdProductDto = _mapper.Map<ProductDto>(product);
                return Result<ProductDto>.Success(createdProductDto);
            }
            else
            {
                _imageStorageService.DeleteImage(product.ImageUrl, rootPath);
                return Result<ProductDto>.Failure(saveResult.Error);
            }
        }
        public async Task<Result<ProductDto>> UpdateProductAsync(
      UpdateProductDto productDto,
      string rootPath)
        {
            var productResult = await _unitOfWork.ProductRepository
                .GetByIdAsync(productDto.Id,
                    include: x => x.Include(x => x.Category));

            if (productResult.IsFailure)
                return Result<ProductDto>.Failure(productResult.Error);

            var product = productResult.Value;

            _mapper.Map(productDto, product);

            string? oldImageUrl = product.ImageUrl;
            string? newImageUrl = null;

            if (productDto.ImageFile != null)
            {
                var uploadResult = await _imageStorageService
                    .SaveImageAsync(productDto.ImageFile, rootPath, "products");

                if (uploadResult.IsFailure)
                    return Result<ProductDto>.Failure(uploadResult.Error);

                newImageUrl = uploadResult.Value;
                product.ImageUrl = newImageUrl;
            }

            var saveResult = await _unitOfWork.SaveChangesAsync();

            if (saveResult.IsFailure)
            {
                if (newImageUrl != null)
                    _imageStorageService.DeleteImage(newImageUrl, rootPath);

                return Result<ProductDto>.Failure(saveResult.Error);
            }

            if (newImageUrl != null && !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                _imageStorageService.DeleteImage(oldImageUrl, rootPath);
            }

            return Result<ProductDto>.Success(
                _mapper.Map<ProductDto>(product));
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
        {
            var productResult = await _unitOfWork.ProductRepository.GetByIdAsync(id, include: x => x.Include(x => x.Category));

            if (productResult.IsFailure)
            {
                return Result<ProductDto>.Failure(productResult.Error);
            }

            var productDto = _mapper.Map<ProductDto>(productResult.Value);
            return Result<ProductDto>.Success(productDto);
        }

        public async Task<Result> DeleteProductAsync(int id, string rootPath)
        {
            var productResult = await _unitOfWork.ProductRepository.GetByIdAsync(id, include: x => x.Include(x => x.Category));

            if (productResult.IsFailure)
            {
                return Result.Failure(productResult.Error);
            }

            var product = productResult.Value;

            var removeResult = _imageStorageService.DeleteImage(product.ImageUrl, rootPath);

            var result = await _unitOfWork.ProductRepository.DeleteAsync(product);

            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult.IsFailure)
            {
                return Result.Failure(saveResult.Error);
            }
            return Result.Success();
        }

        public async Task<Result<PaginatedResult<ProductDto>>> GetProductsPaginatedAsync(
            string search = null,
            int? category = null,
            string sortBy = nameof(Product.Name),
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                Expression<Func<Product, bool>> filter = p =>
                    (string.IsNullOrEmpty(search) || p.Name.Contains(search) || p.Brand.Contains(search)) &&
                    (category == null || p.CategoryId == category);

                Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null;

                if (!string.IsNullOrEmpty(sortBy))
                {
                    orderBy = sortBy.ToLower() switch
                    {
                        "id" => ascending
                            ? q => q.OrderBy(p => p.Id)
                            : q => q.OrderByDescending(p => p.Id),
                        "name" => ascending
                            ? q => q.OrderBy(p => p.Name)
                            : q => q.OrderByDescending(p => p.Name),

                        "brand" => ascending
                            ? q => q.OrderBy(p => p.Brand)
                            : q => q.OrderByDescending(p => p.Brand),

                        "price" => ascending
                            ? q => q.OrderBy(p => p.Price)
                            : q => q.OrderByDescending(p => p.Price),

                        "category" => ascending
                            ? q => q.OrderBy(p => p.Category)
                            : q => q.OrderByDescending(p => p.Category),

                        "createdat" => ascending
                            ? q => q.OrderBy(p => p.CreatedAt)
                            : q => q.OrderByDescending(p => p.CreatedAt),

                        _ => q => q.OrderBy(p => p.Id)
                    };
                }

                var repoResult = await _unitOfWork.ProductRepository.GetPaginatedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    include: x => x.Include(x => x.Category),
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    disableTracking: true
                    );

                if (repoResult.IsFailure)
                    return Result<PaginatedResult<ProductDto>>.Failure(repoResult.Error);

                var productDtos = _mapper.Map<List<ProductDto>>(repoResult.Value.Items);

                var paginatedDtoResult = new PaginatedResult<ProductDto>(
                    productDtos,
                    repoResult.Value.TotalCount,
                    repoResult.Value.PageNumber,
                    repoResult.Value.PageSize);

                return Result<PaginatedResult<ProductDto>>.Success(paginatedDtoResult);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<ProductDto>>.Failure(Error.Failure("Service.Error", ex.Message));
            }
        }
    }
}
