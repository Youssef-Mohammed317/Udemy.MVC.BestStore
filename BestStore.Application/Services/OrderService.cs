using AutoMapper;
using BestStore.Application.DTOs.Cart;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Application.Interfaces.Services;
using BestStore.Shared.Entities;
using BestStore.Shared.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(ICurrentUserService currentUserService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this._currentUserService = currentUserService;
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
        }
        public async Task<Result> CreateOrderAsync(CartDto cartDto)
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
    }
}
