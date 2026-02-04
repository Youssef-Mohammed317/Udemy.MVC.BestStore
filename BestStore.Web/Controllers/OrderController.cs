using AutoMapper;
using BestStore.Application.DTOs.Order;
using BestStore.Application.Interfaces.Services;
using BestStore.Shared.Result;
using BestStore.Web.Models.ViewModels.Order;
using BestStore.Web.Models.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestStore.Web.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IConfiguration configuration,
            IProductService productService, IMapper mapper)
        {
            this._orderService = orderService;
            this._configuration = configuration;
            this._productService = productService;
            this._mapper = mapper;
        }
        public async Task<IActionResult> Index(OrderQueryParams queryParams)
        {

            var result = await _orderService.GetOrdersPaginatedAsync(
                search: queryParams.Search,
                sortBy: queryParams.SortBy,
                ascending: queryParams.Ascending,
                pageNumber: queryParams.PageNumber,
                pageSize: queryParams.PageSize
                );


            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Message;
            }
            var paginatedViewModel = _mapper.Map<PaginatedResult<OrderViewModel>>(result.Value);

            var viewModel = new OrderListViewModel
            {
                PaginatedOrders = paginatedViewModel,
                Search = queryParams.Search,
                SortBy = queryParams.SortBy,
                Ascending = queryParams.Ascending,

            };

            return View(viewModel);
        }


        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderService.GetOrderDetailsByIdAsync(id);
            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Message;
            }
            var ordersViewModel = _mapper.Map<OrderViewModel>(result.Value);

            return View(ordersViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateOrderViewModel updateOrderViewModel)
        {
            var result = await _orderService.UpdateOrderPaymentStatusAsync(_mapper.Map<UpdateOrderDto>(updateOrderViewModel));

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Order Updated Successfully";
            }


            return RedirectToAction("Details", new { id = updateOrderViewModel.Id });
        }
    }
}
