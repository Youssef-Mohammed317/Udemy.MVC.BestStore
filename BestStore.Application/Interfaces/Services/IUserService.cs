using BestStore.Application.DTOs.User;
using BestStore.Domain.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result> ChangeEmailStatusAsync(string id);
        Task<Result> DeleteUserAsync(string id);
        Task<Result<List<RolePairDto>>> GetAllRolesAsync();
        Task<Result<PaginatedResult<UserDto>>> GetAllUsersAsync(string search = null, string sortBy = "CreatedAt", bool ascending = false, int pageNumber = 1, int pageSize = 10);
        Task<Result<UserDto>> GetUserByIdAsync(string id);
        Task<Result> UpdateRoles(string id, string roles);
    }
}
