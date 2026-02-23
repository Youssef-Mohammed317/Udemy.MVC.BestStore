using BestStore.Application.DTOs.Account;
using BestStore.Application.Interfaces.Services;
using BestStore.Domain.Entities;
using BestStore.Domain.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BestStore.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICurrentUserService _currentUser;

        public AccountService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICurrentUserService currentUser
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._currentUser = currentUser;
        }

        public async Task<Result> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return Result.Failure(Error.Failure("Invalid", "Invalid email confirmation link."));
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Result.Failure(Error.Failure("NotFound", "User not found."));
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                return Result.Success();
            }
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(Error.Failure(error.Code, error.Description));
        }

        public async Task<Result<SendConfirmEmailDto>> RegisterAsync(RegisterDto registerDto)
        {
            // check for email
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                return Result<SendConfirmEmailDto>.Failure(Error.Failure("Email", "This Email Already Exist"));
            }


            var user = new ApplicationUser
            {
                AcceptTerms = registerDto.AcceptTerms,
                Address = registerDto.Address,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,

            };


            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();
                return Result<SendConfirmEmailDto>.Failure(Error.Failure(error?.Code ?? "CreateFailed", error?.Description ?? "Failed To Create User"));
            }
            await _userManager.AddToRoleAsync(user, "Customer");

            return Result<SendConfirmEmailDto>.Success(await GetEmailConfirmaitonDtoAsync(user));

        }


        public async Task<Result<EmailConfirmaitonDto>> GetEmailConfirmaionAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<EmailConfirmaitonDto>.Failure(Error.Failure("NotFound", "User not found."));
            }

            return Result<EmailConfirmaitonDto>.Success(new EmailConfirmaitonDto
            {
                Id = userId,
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed
            });
        }

        public async Task<Result<SendConfirmEmailDto>> GetVerficationTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Result<SendConfirmEmailDto>.Failure(Error.Failure("NotFound", "User not found."));
            }


            return Result<SendConfirmEmailDto>.Success(await GetEmailConfirmaitonDtoAsync(user));
        }



        public async Task<Result> LoginAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Result.Success();
            }
            else
            {
                return Result.Failure(Error.Failure("Invalid", "Invalid login attempt."));
            }

        }

        public async Task<Result<ForgotPasswordDto>> ForgotPasswordAsync(string email)
        {

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result<ForgotPasswordDto>.Failure(Error.Failure("NotFound", "User not found."));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return Result<ForgotPasswordDto>.Success(new ForgotPasswordDto { Id = user.Id, Email = user.Email, Token = encodedToken });

        }


        private async Task<SendConfirmEmailDto> GetEmailConfirmaitonDtoAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token)
            );

            return new SendConfirmEmailDto
            {
                Email = user.Email,
                Id = user.Id,
                Token = encodedToken,
            };
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                return Result.Failure(Error.Failure("NotFound", "User not found."));
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                dto.Password
            );

            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return Result.Success();

            }
            return Result.Failure(Error.Failure("Faild", "Failed to reset password"));

        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {


            var user = await _userManager.FindByIdAsync(changePasswordDto.UserId);

            if (user == null)
            {
                return Result.Failure(Error.Failure("NotFound", "User not found."));
            }


            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {
                await _userManager.UpdateAsync(user);

                await _signInManager.RefreshSignInAsync(user);

                return Result.Success();
            }
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(Error.Failure("CurrentPassword", error.Description));
        }

        public async Task<Result<UpdateProfileDto>> GetCurrentUserProfileAsync()
        {
            if (_currentUser?.UserId == null)
            {
                return Result<UpdateProfileDto>.Failure(Error.Failure("NotFound", "User not found."));
            }

            var user = await _userManager.FindByIdAsync(_currentUser.UserId);
            if (user == null)
            {
                return Result<UpdateProfileDto>.Failure(Error.Failure("NotFound", "User not found."));
            }
            var dto = new UpdateProfileDto
            {
                UserId = user.Id,
                Address = user.Address,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return Result<UpdateProfileDto>.Success(dto);

        }

        public async Task<Result<UpdateProfileDto>> UpdateUserProfileAsync(UpdateProfileDto dto)
        {
            if (_currentUser?.UserId == null)
            {
                return Result<UpdateProfileDto>.Failure(Error.Failure("NotFound", "User not found."));
            }

            var user = await _userManager.FindByIdAsync(_currentUser?.UserId ?? "");
            if (user == null)
            {
                return Result<UpdateProfileDto>.Failure(Error.Failure("NotFound", "User not found."));
            }


            if (dto.Email != user.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return Result<UpdateProfileDto>.Failure(Error.Failure("Email", "This email is already in use by another account."));
                }
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            if (dto.Email != user.Email)
            {
                user.EmailConfirmed = false;
            }
            user.Address = dto.Address;
            user.Email = dto.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                dto.CreatedAt = user.CreatedAt;
                return Result<UpdateProfileDto>.Success(dto);
            }

            var error = result.Errors.FirstOrDefault();
            return Result<UpdateProfileDto>.Failure(Error.Failure("CurrentPassword", error.Description));
        }




        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
