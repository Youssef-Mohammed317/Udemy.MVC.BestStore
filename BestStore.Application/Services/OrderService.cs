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
using Microsoft.EntityFrameworkCore.Query;
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
            return await GetOrderInternalAsync(id);
        }
        public async Task<Result<OrderDto>> GetOrderDetailsByIdAsync(int id)
        {
            return await GetOrderInternalAsync(
                id,
                q => q
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                            .ThenInclude(p => p.Category)
            );
        }


        public async Task<Result<OrderDto>> UpdateOrderPaymentStatusAsync(UpdateOrderDto orderDto)
        {
            var orderResult = await _unitOfWork.OrderRepository.GetByIdAsync(orderDto.Id);
            if (orderResult.IsFailure)
            {
                return Result<OrderDto>.Failure(orderResult.Error);
            }

            var order = orderResult.Value;
            order.OrderStatus = orderDto.OrderStatus;

            order.PaymentStatus = orderDto.PaymentStatus;

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult.IsFailure)
            {
                return Result<OrderDto>.Failure(saveResult.Error);
            }

            return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
        }

        public async Task<Result<PaginatedResult<OrderDto>>> GetOrdersPaginatedAsync(
            string search = null,
            string sortBy = nameof(Order.CreatedAt),
            bool ascending = false,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                Expression<Func<Order, bool>> filter = p =>
                    (string.IsNullOrEmpty(search) || p.DeliveryAddress.Contains(search)
                    || p.PaymentMethod.Contains(search) || p.PaymentDetails.Contains(search)
                    || p.OrderStatus.Contains(search) || p.PaymentStatus.Contains(search));
                Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = null;

                if (!string.IsNullOrEmpty(sortBy))
                {
                    orderBy = sortBy.ToLower() switch
                    {
                        "id" => ascending
                            ? q => q.OrderBy(p => p.Id)
                            : q => q.OrderByDescending(p => p.Id),
                        "deliveryaddress" => ascending
                            ? q => q.OrderBy(p => p.DeliveryAddress)
                            : q => q.OrderByDescending(p => p.DeliveryAddress),

                        "paymentmethod" => ascending
                            ? q => q.OrderBy(p => p.PaymentMethod)
                            : q => q.OrderByDescending(p => p.PaymentMethod),
                        "paymentstatus" => ascending
                            ? q => q.OrderBy(p => p.PaymentStatus)
                            : q => q.OrderByDescending(p => p.PaymentStatus),

                        "paymentdetails" => ascending
                            ? q => q.OrderBy(p => p.PaymentDetails)
                            : q => q.OrderByDescending(p => p.PaymentDetails),

                        "orderstatus" => ascending
                            ? q => q.OrderBy(p => p.OrderStatus)
                            : q => q.OrderByDescending(p => p.OrderStatus),

                        "createdat" => ascending
                            ? q => q.OrderBy(p => p.CreatedAt)
                            : q => q.OrderByDescending(p => p.CreatedAt),

                        _ => q => q.OrderByDescending(p => p.CreatedAt)
                    };
                }

                var repoResult = await _unitOfWork.OrderRepository.GetPaginatedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    include: null,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    disableTracking: true
                    );

                if (repoResult.IsFailure)
                    return Result<PaginatedResult<OrderDto>>.Failure(repoResult.Error);

                var productDtos = _mapper.Map<List<OrderDto>>(repoResult.Value.Items);

                var paginatedDtoResult = new PaginatedResult<OrderDto>(
                    productDtos,
                    repoResult.Value.TotalCount,
                    repoResult.Value.PageNumber,
                    repoResult.Value.PageSize);

                return Result<PaginatedResult<OrderDto>>.Success(paginatedDtoResult);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<OrderDto>>.Failure(Error.Failure("Service.Error", ex.Message));
            }
        }
        private async Task<Result<OrderDto>> GetOrderInternalAsync(
           int id,
           Func<IQueryable<Order>, IIncludableQueryable<Order, object>>? include = null)
        {

            var orderResult = await _unitOfWork.OrderRepository.GetByIdAsync(id, include: include);



            if (orderResult.IsFailure || orderResult.Value == null)
                return Result<OrderDto>.Failure(orderResult.Error);

            var order = orderResult.Value;

            var orderDto = _mapper.Map<OrderDto>(order);

            var userResult = await _userService.GetUserByIdAsync(order.ClientId);
            if (userResult.IsFailure)
                return Result<OrderDto>.Failure(userResult.Error);

            orderDto.Client = userResult.Value;

            return Result<OrderDto>.Success(orderDto);
        }



    }
}
