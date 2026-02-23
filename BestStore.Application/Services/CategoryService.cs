using AutoMapper;
using BestStore.Application.DTOs.Category;
using BestStore.Application.DTOs.Category;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Application.Interfaces.Services;
using BestStore.Application.Interfaces.Utility;
using BestStore.Domain.Entities;
using BestStore.Domain.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<Result<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            var result = await _unitOfWork.CategoryRepository.GetAllCategoriesAsync();
            if (result.IsFailure)
            {
                return Result<List<CategoryDto>>.Failure(result.Error);
            }
            var categoryDtos = _mapper.Map<List<CategoryDto>>(result.Value);

            return Result<List<CategoryDto>>.Success(categoryDtos);
        }
        public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);


            await _unitOfWork.CategoryRepository.AddAsync(category);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result.IsSuccess)
            {
                var createdCategoryDto = _mapper.Map<CategoryDto>(category);
                return Result<CategoryDto>.Success(createdCategoryDto);
            }
            else
            {
                return Result<CategoryDto>.Failure(result.Error);
            }
        }
        public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var result = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (result.IsFailure)
            {
                return Result<CategoryDto>.Failure(result.Error);
            }


            var categoryDto = _mapper.Map<CategoryDto>(result.Value);
            return Result<CategoryDto>.Success(categoryDto);

        }
        public async Task<Result<CategoryDto>> UpdateCategoryAsync(UpdateCategoryDto categoryDto)
        {
            var result = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryDto.Id);

            if (result.IsFailure)
            {
                return Result<CategoryDto>.Failure(result.Error);
            }

            var category = result.Value;

            _mapper.Map(categoryDto, category);

            await _unitOfWork.CategoryRepository.UpdateAsync(category);
            var res = await _unitOfWork.SaveChangesAsync();

            if (res.IsSuccess)
            {
                var updatedCategoryDto = _mapper.Map<CategoryDto>(category);
                return Result<CategoryDto>.Success(updatedCategoryDto);
            }
            else
            {
                return Result<CategoryDto>.Failure(result.Error);
            }
        }


        public async Task<Result> DeleteCategoryAsync(int id)
        {
            var result = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }


            await _unitOfWork.CategoryRepository.DeleteAsync(result.Value);
            var res = await _unitOfWork.SaveChangesAsync();

            if (res.IsSuccess)
            {
                return Result.Success();
            }
            else
            {
                return Result.Failure(result.Error);
            }
        }

    }

}
