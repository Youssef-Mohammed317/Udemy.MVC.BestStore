using AutoMapper;
using BestStore.Application.DTOs.Category;
using BestStore.Application.Interfaces.Services;
using BestStore.Domain.Entities;
using BestStore.Domain.Result;
using BestStore.Web.Models.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestStore.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("/admin/[controller]/[action]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly string _errorMessageKey = "ErrorMessage";
        private readonly string _successMessageKey = "SuccessMessage";
        private readonly string _indexKey = "Index";

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            this._categoryService = categoryService;
            this._mapper = mapper;
        }


        public async Task<IActionResult> Index()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            if (result.IsSuccess)
            {
                TempData[_successMessageKey] = "Successfully Get Data";

                var viewModels = _mapper.Map<List<CategoryViewModel>>(result.Value);

                return View(viewModels);
            }
            else
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return View();
            }
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var categoryDto = _mapper.Map<CreateCategoryDto>(model);

                var result = await _categoryService.CreateCategoryAsync(categoryDto);

                if (result.IsSuccess)
                {
                    TempData[_successMessageKey] = "Category created successfully.";
                    return RedirectToAction(_indexKey);
                }
                else
                {
                    ModelState.AddModelError(result.Error.Code, result.Error.Message);
                    TempData[_errorMessageKey] = result.Error.Message;
                }
            }

            return View(model);

        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction(_indexKey);
            }
            var categoryModel = _mapper.Map<UpdateCategoryViewModel>(result.Value);


            return View(categoryModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var categoryDto = _mapper.Map<UpdateCategoryDto>(model);

                var result = await _categoryService.UpdateCategoryAsync(categoryDto);

                if (result.IsSuccess)
                {
                    TempData[_successMessageKey] = "Category updated successfully.";

                    return RedirectToAction(_indexKey);
                }
                ModelState.AddModelError(result.Error.Code, result.Error.Message);
                TempData[_errorMessageKey] = result.Error.Message;
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result.IsSuccess)
            {
                TempData[_successMessageKey] = "Category deleted successfully.";
            }
            else
            {
                TempData[_errorMessageKey] = result.Error.Message;
            }
            return RedirectToAction(_indexKey);
        }
    }
}
