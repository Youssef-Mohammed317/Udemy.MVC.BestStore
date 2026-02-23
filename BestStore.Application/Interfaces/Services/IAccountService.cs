using BestStore.Application.DTOs.Account;
using BestStore.Domain.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<Result> ConfirmEmailAsync(string userId, string token);
        Task<Result<SendConfirmEmailDto>> RegisterAsync(RegisterDto registerDto);
        Task<Result<SendConfirmEmailDto>> GetVerficationTokenAsync(string userId);
        Task<Result<EmailConfirmaitonDto>> GetEmailConfirmaionAsync(string userId);
        Task<Result> LoginAsync(LoginDto loginDto);
        Task<Result<ForgotPasswordDto>> ForgotPasswordAsync(string email);
        Task<Result> ResetPasswordAsync(ResetPasswordDto dto);
        Task<Result> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<Result<UpdateProfileDto>> GetCurrentUserProfileAsync();
        Task<Result<UpdateProfileDto>> UpdateUserProfileAsync(UpdateProfileDto dto);
        Task LogoutAsync();
    }
}
