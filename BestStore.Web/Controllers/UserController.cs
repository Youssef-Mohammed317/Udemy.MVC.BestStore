using AutoMapper;
using BestStore.Application.DTOs.User;
using BestStore.Application.Interfaces.Services;
using BestStore.Domain.Result;
using BestStore.Web.Models.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace BestStore.Web.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Route("/admin/[controller]/{action=Index}/{id?}")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper; private readonly string _errorMessageKey = "ErrorMessage";
        private readonly string _successMessageKey = "SuccessMessage";

        public UserController(IUserService userService, IMapper mapper)
        {
            this._userService = userService;
            this._mapper = mapper;
        }
        public async Task<IActionResult> Index(UserQueryParams userQueryParams)
        {
            var result = await _userService.GetAllUsersAsync(
                search: userQueryParams.Search,
                sortBy: userQueryParams.SortBy,
                ascending: userQueryParams.Ascending,
                pageNumber: userQueryParams.PageNumber,
                pageSize: userQueryParams.PageSize
                );

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = "Somthing went wrong";
                return RedirectToAction("Index", "Home");
            }
            TempData[_successMessageKey] = "Users retrieved successfully.";

            var users = _mapper.Map<PaginatedResult<UserViewModel>>(result.Value);

            var viewModel = new UserListViewModel
            {
                PaginatedUsers = users,
                Search = userQueryParams.Search,
                SortBy = userQueryParams.SortBy,
                Ascending = userQueryParams.Ascending,
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeEmailStatus(string id)
        {
            var result = await _userService.ChangeEmailStatusAsync(id);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
            }
            else
            {
                TempData[_successMessageKey] = "Successfully Changed Email Status";
            }
            
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Details(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction("Index", "Home");
            }

            var userView = _mapper.Map<UserDetailsViewModel>(result.Value);

            return View(userView);

        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
            }
            TempData[_successMessageKey] = "user deleted successfully";
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> ManageRoles(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction("Index", "Home");
            }
            var roles = await _userService.GetAllRolesAsync();

            var manageRolesView = _mapper.Map<ManageRolesViewModel>(result.Value);

            manageRolesView.SystemRolesPairs = _mapper.Map<List<RolePairViewModel>>(roles.Value);

            return View(manageRolesView);
        }
        [HttpPost]
        public async Task<IActionResult> ManageRoles(string id, string SelectedRoles)
        {

            var result = await _userService.UpdateRoles(id, SelectedRoles);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
            }
            else
            {

                TempData[_successMessageKey] = "Successfully Roles Updated";
            }



            return RedirectToAction("ManageRoles", new { id });
        }


    }
}
