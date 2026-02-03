using AutoMapper;
using BestStore.Application.Interfaces.Services;
using BestStore.Web.Models.ViewModels.Order;
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
        public async Task<IActionResult> Index()
        {
            var result = await _orderService.GetAllOrdersAsync();
            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Message;
            }
            var ordersViewModel = _mapper.Map<List<OrderViewModel>>(result.Value);

            return View(ordersViewModel);
        }
        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Message;
            }
            var ordersViewModel = _mapper.Map<OrderViewModel>(result.Value);

            return View(ordersViewModel);
        }
    }
}
