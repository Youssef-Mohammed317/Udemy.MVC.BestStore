using AutoMapper;
using BestStore.Application.DTOs.Cart;
using BestStore.Application.DTOs.Order;
using BestStore.Application.Interfaces.Repositories;
using BestStore.Application.Interfaces.Services;
using BestStore.Shared.Entities;
using BestStore.Shared.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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
        public async Task<Result> CreateCartOrderAsync(CartDto cartDto, bool IsPaypalAccepted = false)
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
                PaymentStatus = IsPaypalAccepted ? "accepted" : "pending",
                PaymentDetails = cartDto.CheckoutDto.PaymentDetails,
                OrderStatus = "created",
            };


            var addResult = await _unitOfWork.OrderRepository.AddAsync(order);
            if (addResult.IsFailure)
            {
                return Result.Failure(addResult.Error);
            }

            foreach (var item in cartDto.CartItems)
            {
                var entityResult = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (entityResult.IsFailure)
                {
                    return Result.Failure(entityResult.Error);
                }
                var entity = entityResult.Value;

                if (entity.StockQuantity > 0 && entity.StockQuantity >= item.Quantity)
                {
                    entity.StockQuantity -= item.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(entity);
                }
            }

            var result = await _unitOfWork.SaveChangesAsync();
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }


            return Result.Success();
        }


        public async Task<Result<OrderDetailsDto>> GetOrderByIdAsync(int id)
        {
            return await GetOrderInternalAsync(id);
        }
        public async Task<Result<OrderDetailsDto>> GetOrderDetailsByIdAsync(int id)
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
        string? search = null,
        string sortBy = nameof(Order.CreatedAt),
        bool ascending = false,
        int pageNumber = 1,
        int pageSize = 10,
        string? userId = null,
        bool isAdmin = false
)
        {
            Expression<Func<Order, bool>> filter = p =>
                (string.IsNullOrEmpty(search)
                    || p.DeliveryAddress.Contains(search)
                    || p.PaymentMethod.Contains(search)
                    || p.PaymentDetails.Contains(search)
                    || p.OrderStatus.Contains(search)
                    || p.PaymentStatus.Contains(search))
                && (isAdmin || p.ClientId == userId);

            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy =
                GetOrderBy(sortBy, ascending);

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



            var orderDtos = _mapper.Map<List<OrderDto>>(repoResult.Value.Items);


            foreach (var orderDto in orderDtos)
            {
                var userResult = await _userService.GetUserByIdAsync(orderDto.ClientId);

                if (userResult.IsFailure)
                    continue;

                orderDto.Client = userResult.Value;
            }


            var paginatedDtoResult = new PaginatedResult<OrderDto>(
                orderDtos,
                repoResult.Value.TotalCount,
                repoResult.Value.PageNumber,
                repoResult.Value.PageSize
            );

            return Result<PaginatedResult<OrderDto>>.Success(paginatedDtoResult);
        }

        private async Task<Result<OrderDetailsDto>> GetOrderInternalAsync(int id, Func<IQueryable<Order>, IIncludableQueryable<Order, object>>? include = null)
        {

            var orderResult = await _unitOfWork.OrderRepository.GetByIdAsync(id, include: include!);



            if (orderResult.IsFailure || orderResult.Value == null)
                return Result<OrderDetailsDto>.Failure(orderResult.Error);

            var order = orderResult.Value;

            var orderDto = _mapper.Map<OrderDetailsDto>(order);

            var userResult = await _userService.GetUserByIdAsync(order.ClientId);
            if (userResult.IsFailure)
                return Result<OrderDetailsDto>.Failure(userResult.Error);

            orderDto.Client = userResult.Value;
            orderDto.ClientOrdersCount = await _unitOfWork.OrderRepository.GetNumberOfOrderByUserId(order.ClientId);

            return Result<OrderDetailsDto>.Success(orderDto);
        }
        private static Func<IQueryable<Order>, IOrderedQueryable<Order>> GetOrderBy(string sortBy, bool ascending)
        {
            return sortBy.ToLower() switch
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



    }
}
