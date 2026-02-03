using AutoMapper;
using BestStore.Application.DTOs.Cart;
using BestStore.Application.DTOs.Order;
using BestStore.Application.DTOs.Product;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Application.Interfaces.Services;
using BestStore.Application.Interfaces.Utility;
using BestStore.Shared.Entities;
using BestStore.Shared.Result;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BestStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(ICurrentUserService currentUserService, IUserService userService,
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            this._currentUserService = currentUserService;
            this._userService = userService;
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
        }
        public async Task<Result> CreateCartOrderAsync(CartDto cartDto)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result.Failure(Error.Failure("Auth", "You must be auth"));
            }
            var order = new Order
            {
                ClientId = userId,
                Items = _mapper.Map<List<OrderItem>>(cartDto.CartItems),
                ShippingFee = cartDto.ShippingFee,
                DeliveryAddress = cartDto.CheckoutDto.DeliveryAddress,
                PaymentMethod = cartDto.CheckoutDto.PaymentMethod,
                PaymentStatus = "pending",
                PaymentDetails = "",
                OrderStatus = "created",
            };

            var result = await _unitOfWork.OrderRepository.AddAsync(order);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
            result = await _unitOfWork.SaveChangesAsync();
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
            return Result.Success();
        }


        public async Task<Result<List<OrderDto>>> GetAllOrdersAsync()
        {
            var orderResult = await _unitOfWork.OrderRepository.GetAllAsync();

            if (orderResult.IsFailure)
                return Result<List<OrderDto>>.Failure(orderResult.Error);


            List<OrderDto> result = new List<OrderDto>();

            foreach (var order in orderResult.Value)
            {
                var orderDto = _mapper.Map<OrderDto>(order);
                var userResult = await _userService.GetUserByIdAsync(orderDto.ClientId);

                if (userResult.IsFailure)
                    continue;

                orderDto.Client = userResult.Value;
                result.Add(orderDto);
            }

            return Result<List<OrderDto>>.Success(result.OrderByDescending(c => c.CreatedAt).ToList());
        }
        public async Task<Result<OrderDto>> GetOrderByIdAsync(int id)
        {
            var orderResult = await _unitOfWork.OrderRepository.GetByIdAsync(id);

            if (orderResult.IsFailure)
                return Result<OrderDto>.Failure(orderResult.Error);


            var orderDto = _mapper.Map<OrderDto>(orderResult.Value);
            var userResult = await _userService.GetUserByIdAsync(orderResult.Value.ClientId);
            if (userResult.IsFailure)
                return Result<OrderDto>.Failure(userResult.Error);
            orderDto.Client = userResult.Value;

            return Result<OrderDto>.Success(orderDto);
        }


        public async Task<Result<OrderDto>> UpdateOrderAsync(UpdateOrderDto orderDto)
        {
            var orderResult = await _unitOfWork.OrderRepository.GetByIdAsync(orderDto.Id);
            if (orderResult.IsFailure)
            {
                return Result<OrderDto>.Failure(orderResult.Error);
            }
            var order = orderResult.Value;
            _mapper.Map(orderDto, order);

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult.IsFailure)
            {
                return Result<OrderDto>.Failure(saveResult.Error);
            }

            return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
        }

        //public async Task<Result<OrderDto>> GetProductByIdAsync(int id)
        //{
        //    var productResult = await _unitOfWork.ProductRepository.GetByIdAsync(id, include: x => x.Include(x => x.Category));

        //    if (productResult.IsFailure)
        //    {
        //        return Result<OrderDto>.Failure(productResult.Error);
        //    }

        //    var productDto = _mapper.Map<OrderDto>(productResult.Value);
        //    return Result<OrderDto>.Success(productDto);
        //}

        //public async Task<Result> DeleteProductAsync(int id)
        //{
        //    var productResult = await _unitOfWork.ProductRepository.GetByIdAsync(id, include: x => x.Include(x => x.Category));
        //    if (productResult.IsFailure)
        //    {
        //        return Result.Failure(productResult.Error);
        //    }
        //    var product = productResult.Value;

        //    var result = await _unitOfWork.ProductRepository.DeleteAsync(product);

        //    if (result.IsFailure)
        //    {
        //        return Result.Failure(result.Error);
        //    }

        //    var saveResult = await _unitOfWork.SaveChangesAsync();
        //    if (saveResult.IsFailure)
        //    {
        //        return Result.Failure(saveResult.Error);
        //    }
        //    return Result.Success();
        //}

        //public async Task<Result<PaginatedResult<OrderDto>>> GetOrdersPaginatedAsync(
        //    string search = null,
        //    int? category = null,
        //    string sortBy = nameof(Product.Name),
        //    bool ascending = true,
        //    int pageNumber = 1,
        //    int pageSize = 10)
        //{
        //    try
        //    {
        //        Expression<Func<Product, bool>> filter = p =>
        //            (string.IsNullOrEmpty(search) || p.Name.Contains(search) || p.Brand.Contains(search)) &&
        //            (category == null || p.CategoryId == category);

        //        Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null;

        //        if (!string.IsNullOrEmpty(sortBy))
        //        {
        //            orderBy = sortBy.ToLower() switch
        //            {
        //                "id" => ascending
        //                    ? q => q.OrderBy(p => p.Id)
        //                    : q => q.OrderByDescending(p => p.Id),
        //                "name" => ascending
        //                    ? q => q.OrderBy(p => p.Name)
        //                    : q => q.OrderByDescending(p => p.Name),

        //                "brand" => ascending
        //                    ? q => q.OrderBy(p => p.Brand)
        //                    : q => q.OrderByDescending(p => p.Brand),

        //                "price" => ascending
        //                    ? q => q.OrderBy(p => p.Price)
        //                    : q => q.OrderByDescending(p => p.Price),

        //                "category" => ascending
        //                    ? q => q.OrderBy(p => p.Category)
        //                    : q => q.OrderByDescending(p => p.Category),

        //                "createdat" => ascending
        //                    ? q => q.OrderBy(p => p.CreatedAt)
        //                    : q => q.OrderByDescending(p => p.CreatedAt),

        //                _ => q => q.OrderBy(p => p.Id)
        //            };
        //        }

        //        var repoResult = await _unitOfWork.ProductRepository.GetPaginatedAsync(
        //            filter: filter,
        //            orderBy: orderBy,
        //            include: x => x.Include(x => x.Category),
        //            pageNumber: pageNumber,
        //            pageSize: pageSize,
        //            disableTracking: true
        //            );

        //        if (repoResult.IsFailure)
        //            return Result<PaginatedResult<OrderDto>>.Failure(repoResult.Error);

        //        var productDtos = _mapper.Map<List<OrderDto>>(repoResult.Value.Items);

        //        var paginatedDtoResult = new PaginatedResult<OrderDto>(
        //            productDtos,
        //            repoResult.Value.TotalCount,
        //            repoResult.Value.PageNumber,
        //            repoResult.Value.PageSize);

        //        return Result<PaginatedResult<OrderDto>>.Success(paginatedDtoResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result<PaginatedResult<OrderDto>>.Failure(Error.Failure("Service.Error", ex.Message));
        //    }
        //}

    }
}
