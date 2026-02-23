using BestStore.Application.DTOs.User;
using BestStore.Application.Interfaces.Services;
using BestStore.Domain.Entities;
using BestStore.Domain.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace BestStore.Infrastructure.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationUser _currentUser;
        private readonly bool _isSuperAdmin;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ICurrentUserService currentUserService
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            _currentUser = _userManager.FindByIdAsync(currentUserService?.UserId!).GetAwaiter().GetResult()!;
            if (_currentUser != null)
                _isSuperAdmin = _userManager.IsInRoleAsync(_currentUser, "SuperAdmin").GetAwaiter().GetResult();
        }

        public async Task<Result<PaginatedResult<UserDto>>> GetAllUsersAsync(
                string search = null,
                string sortBy = nameof(ApplicationUser.CreatedAt),
                bool ascending = false,
                int pageNumber = 1,
                int pageSize = 10)
        {
            try
            {


                Expression<Func<ApplicationUser, bool>> filter = p =>
                      (string.IsNullOrEmpty(search) || p.FirstName.Contains(search) || p.LastName.Contains(search) || p.Email.Contains(search));
                Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderBy = null;
                if (!string.IsNullOrEmpty(sortBy))
                {
                    orderBy = sortBy.ToLower() switch
                    {
                        "id" => ascending
                            ? q => q.OrderBy(p => p.Id)
                            : q => q.OrderByDescending(p => p.Id),
                        "name" => ascending
                            ? q => q.OrderBy(p => p.FirstName)
                            : q => q.OrderByDescending(p => p.FirstName),

                        "email" => ascending
                            ? q => q.OrderBy(p => p.Email)
                            : q => q.OrderByDescending(p => p.Email),

                        "emailconfirmed" => ascending
                            ? q => q.OrderBy(p => p.EmailConfirmed)
                            : q => q.OrderByDescending(p => p.EmailConfirmed),

                        "phonenumber" => ascending
                            ? q => q.OrderBy(p => p.PhoneNumber)
                            : q => q.OrderByDescending(p => p.PhoneNumber),

                        "address" => ascending
                            ? q => q.OrderBy(p => p.Address)
                            : q => q.OrderByDescending(p => p.Address),

                        "createdat" => ascending
                            ? q => q.OrderBy(p => p.CreatedAt)
                            : q => q.OrderByDescending(p => p.CreatedAt),
                        "lastupdatedat" => ascending
                            ? q => q.OrderBy(p => p.LastUpdatedAt)
                            : q => q.OrderByDescending(p => p.LastUpdatedAt),

                        _ => q => q.OrderByDescending(p => p.CreatedAt)
                    };

                }




                var query = _userManager.Users.AsQueryable();

                int totalCount = await query.CountAsync();

                if (filter != null)
                    query = query.Where(filter);


                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                var users = await query.ToListAsync();



                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await GetUserRolesAsync(user);

                    if (!_isSuperAdmin && (roles.Contains("SuperAdmin") || roles.Contains("Admin")) && (user.Id != _currentUser?.Id))
                    {

                        continue;

                    }

                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        CreatedAt = user.CreatedAt,
                        EmailConfirmed = user.EmailConfirmed,
                        LastUpdatedAt = user.LastUpdatedAt,
                        Roles = roles
                    });
                }

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);


                return Result<PaginatedResult<UserDto>>.Success(new PaginatedResult<UserDto>
                {
                    Items = userDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,

                });

            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<UserDto>>.Failure(Error.Failure("UserService.GetAllUsers", ex.Message));
            }
        }

        public async Task<Result> ChangeEmailStatusAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Result.Failure(Error.Failure("UserNotFound", "Failed to find this user"));
            }

            var roles = await GetUserRolesAsync(user);

            if (!_isSuperAdmin && roles.Contains("Admin"))
            {
                return Result.Failure(Error.Failure("AccessDenied", "This Action Can't Done by admin role"));
            }

            user.EmailConfirmed = !user.EmailConfirmed;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Result.Success();
            }
            else
            {
                return Result.Failure(Error.Failure("Error", "Failed to change status of this user"));
            }

        }
        public async Task<Result<UserDto>> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Result<UserDto>.Failure(Error.Failure("UserNotFound", "Failed to find this user"));
            }

            var userDto = new UserDto
            {

                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                EmailConfirmed = user.EmailConfirmed,
                LastUpdatedAt = user.LastUpdatedAt,
                Roles = await GetUserRolesAsync(user),
            };

            return Result<UserDto>.Success(userDto);

        }

        public async Task<Result> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Result.Failure(Error.Failure("UserNotFound", "Failed to find this user"));
            }

            var roles = await GetUserRolesAsync(user);

            if (!_isSuperAdmin && roles.Contains("Admin") && (user.Id != _currentUser?.Id))
            {
                return Result.Failure(Error.Failure("AccessDenied", "This Action Can't Done by admin role"));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Result.Success();
            }
            return Result.Failure(Error.Failure("", "Failed to delete this user"));
        }
        public async Task<Result<List<RolePairDto>>> GetAllRolesAsync()
        {
            var query = _roleManager.Roles;
            if (!_isSuperAdmin)
            {
                query = query.Where(r => !r.Name.ToLower().Contains("superadmin"));
            }


            var roles = await query.Select(r => new RolePairDto
            {
                Id = r.Id,
                RoleName = r.Name
            }
            ).ToListAsync();

            return Result<List<RolePairDto>>.Success(roles);
        }

        public async Task<Result> UpdateRoles(string id, string roles)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Result.Failure(Error.Failure("UserNotFound", "Failed to find this user"));
            }
            var oldRoles = await _userManager.GetRolesAsync(user);

            if (!_isSuperAdmin && oldRoles.Contains("SuperAdmin"))
            {
                return Result.Failure(Error.Failure("AccessDenied", "This Action Can't Done by admin role"));
            }

            var newRoles = roles.Split(",");

            if (newRoles.Contains("SuperAdmin"))
            {
                return Result.Failure(Error.Failure("AccessDenied", "The app must have only one super admin"));
            }

            if (!_isSuperAdmin && oldRoles.Contains("Admin"))
            {
                return Result.Failure(Error.Failure("AccessDenied", "this an admin user as you so contact with the owner"));
            }
            foreach (var role in oldRoles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }
            foreach (var role in newRoles)
            {
                if (await _roleManager.RoleExistsAsync(role.Trim()))
                {
                    await _userManager.AddToRoleAsync(user, role.Trim());
                }
            }
            return Result.Success();
        }




        private async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }


    }
}
