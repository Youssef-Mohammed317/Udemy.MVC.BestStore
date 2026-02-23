using AutoMapper;
using BestStore.Application.DTOs.Product;
using BestStore.Application.Interfaces.Services;
using BestStore.Domain.Result;
using BestStore.Web.Models.ViewModels.Category;
using BestStore.Web.Models.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        private readonly string _errorMessageKey = "ErrorMessage";
        private readonly string _successMessageKey = "SuccessMessage";
        private readonly string _indexKey = "Index";

        public ProductController(IProductService productService, IMapper mapper, ICategoryService categoryService, IWebHostEnvironment env)
        {
            this._productService = productService;
            this._mapper = mapper;
            this._categoryService = categoryService;
            this._env = env;
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("/admin/[controller]")]
        public async Task<IActionResult> Index(ProductQueryParams queryParams)
        {
            var result = await GetProductListViewModelAsync(queryParams);
            if (result.IsSuccess)
            {
                TempData[_successMessageKey] = "Successfully Get Data";
                return View(result.Value);
            }
            TempData[_errorMessageKey] = result.Error.Message;
            return View(new ProductListViewModel());

        }

        public async Task<IActionResult> Store(ProductQueryParams queryParams)
        {
            var result = await GetProductListViewModelAsync(queryParams);
            if (result.IsSuccess)
            {
                TempData[_successMessageKey] = "Successfully Get Data";
                return View(result.Value);
            }
            TempData[_errorMessageKey] = result.Error.Message;
            return View(new ProductListViewModel());

        }
        public async Task<IActionResult> Details([FromRoute] int id)
        {


            var result = await _productService.GetProductDetailsByIdAsync(id);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction(_indexKey);
            }

            var productModel = _mapper.Map<ProductDetailsViewModel>(result.Value);

            return View(productModel);


        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            var categoryPairsResult = await GetCategorySelectPairViewModelAsync();

            if (categoryPairsResult.IsFailure)
            {
                TempData[_errorMessageKey] = categoryPairsResult.Error.Message;
                return RedirectToAction(_indexKey);
            }
            var model = new CreateProductViewModel
            {
                CategorySelectPairs = categoryPairsResult.Value
            };

            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var productDto = _mapper.Map<CreateProductDto>(model);

                var result = await _productService.CreateProductAsync(productDto, _env.WebRootPath);

                if (result.IsSuccess)
                {
                    TempData[_successMessageKey] = "Product created successfully.";
                    return RedirectToAction(_indexKey);
                }
                ModelState.AddModelError(result.Error.Code, result.Error.Message);
                TempData[_errorMessageKey] = result.Error.Message;
            }
            var categoryDtosResult = await _categoryService.GetAllCategoriesAsync();

            if (categoryDtosResult.IsFailure)
            {
                TempData[_errorMessageKey] = "Error loading categories";
                return View(_indexKey);
            }

            model.CategorySelectPairs = _mapper.Map<List<CategorySelectPairViewModel>>(categoryDtosResult.Value);


            return View(model);

        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction(_indexKey);
            }
            var productModel = _mapper.Map<UpdateProductViewModel>(result.Value);


            var categoryPairsResult = await GetCategorySelectPairViewModelAsync();

            if (categoryPairsResult.IsFailure)
            {
                TempData[_errorMessageKey] = categoryPairsResult.Error.Message;
                return RedirectToAction(_indexKey);
            }

            productModel.CategorySelectPairs = categoryPairsResult.Value;

            return View(productModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(UpdateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var productDto = _mapper.Map<UpdateProductDto>(model);

                var result = await _productService.UpdateProductAsync(productDto, _env.WebRootPath);

                if (result.IsSuccess)
                {
                    TempData[_successMessageKey] = "Product updated successfully.";

                    return RedirectToAction(_indexKey);
                }
                ModelState.AddModelError(result.Error.Code, result.Error.Message);
                TempData[_errorMessageKey] = result.Error.Message;
            }
            var categoryPairsResult = await GetCategorySelectPairViewModelAsync();

            if (categoryPairsResult.IsFailure)
            {
                TempData[_errorMessageKey] = categoryPairsResult.Error.Message;
                return RedirectToAction(_indexKey);
            }

            model.CategorySelectPairs = categoryPairsResult.Value;
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id, _env.WebRootPath);
            if (result.IsSuccess)
            {
                TempData[_successMessageKey] = "Product deleted successfully.";
            }
            else
            {
                TempData[_errorMessageKey] = result.Error.Message;
            }
            return RedirectToAction(_indexKey);
        }


        private async Task<Result<ProductListViewModel>> GetProductListViewModelAsync(ProductQueryParams queryParams)
        {
            var result = await _productService.GetProductsPaginatedAsync(
             queryParams.Search,
             queryParams.CategoryId,
             queryParams.SortBy,
             queryParams.Ascending,
             queryParams.PageNumber,
             queryParams.PageSize);

            if (!result.IsSuccess)
            {
                return Result<ProductListViewModel>.Failure(Error.Failure("Error", "Error loading products"));
            }
            var paginatedDto = result.Value;

            var paginatedViewModel = _mapper.Map<PaginatedResult<ProductViewModel>>(paginatedDto);

            var categoryDtosResult = await _categoryService.GetAllCategoriesAsync();

            if (categoryDtosResult.IsFailure)
            {
                return Result<ProductListViewModel>.Failure(Error.Failure("Error", "Error loading categories")); ;
            }

            var categoryPairs = _mapper.Map<List<CategorySelectPairViewModel>>(categoryDtosResult.Value);

            var viewModel = new ProductListViewModel
            {
                PaginatedProducts = paginatedViewModel,
                Search = queryParams.Search,
                CategoryId = queryParams.CategoryId,
                SortBy = queryParams.SortBy,
                Ascending = queryParams.Ascending,
                CategorySelectPairs = categoryPairs
            };
            return Result<ProductListViewModel>.Success(viewModel);
        }
        private async Task<Result<List<CategorySelectPairViewModel>>> GetCategorySelectPairViewModelAsync()
        {

            var categoryDtosResult = await _categoryService.GetAllCategoriesAsync();

            if (categoryDtosResult.IsFailure)
            {
                return Result<List<CategorySelectPairViewModel>>.Failure(Error.Failure("Error", "Error loading categories"));
            }

            return Result<List<CategorySelectPairViewModel>>.Success(_mapper.Map<List<CategorySelectPairViewModel>>(categoryDtosResult.Value));

        }
    }
}
