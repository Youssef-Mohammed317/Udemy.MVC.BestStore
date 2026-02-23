using AutoMapper;
using BestStore.Application.DTOs.Account;
using BestStore.Application.Interfaces.Services;
using BestStore.Web.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using IEmailSender = BestStore.Application.Interfaces.Services.IEmailSender;

namespace BestStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailSender _emailService;
        private readonly IAccountService _accountService;
        private readonly string _errorMessageKey = "ErrorMessage";
        private readonly string _successMessageKey = "SuccessMessage";
        private readonly IMapper _mapper;

        public AccountController(
            IEmailSender emailService,
            IAccountService accountService,
             IMapper mapper)
        {
            this._emailService = emailService;
            this._accountService = accountService;
            this._mapper = mapper;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var registerDto = _mapper.Map<RegisterDto>(model);

                var result = await _accountService.RegisterAsync(registerDto);

                if (result.IsFailure)
                {

                    if (result.Error.Code == "Email")
                    {
                        ModelState.AddModelError("Email", result.Error.Message);
                    }
                    else
                    {

                        TempData[_errorMessageKey] = result.Error.Message;
                    }

                    return View(model);
                }

                var emailResult = await SendConfirmEmailAsync(_mapper.Map<SendConfirmEmailViewModel>(result.Value));

                if (emailResult.IsSuccess)
                {
                    TempData[_successMessageKey] = "Registration successful. Please check your email to confirm your account.";
                }
                else
                {
                    TempData[_successMessageKey] = "Registration successful, but failed to send confirmation email.";
                }

                return RedirectToAction("EmailConfirmation", new { userId = result.Value.Id });

            }
            return View(model);

        }
        public async Task<IActionResult> EmailConfirmation(string userId)
        {
            var result = await _accountService.GetEmailConfirmaionAsync(userId);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction("Register");
            }

            if (!result.Value.IsEmailConfirmed)
            {
                return View(_mapper.Map<EmailConfirmaitonViewModel>(result.Value));
            }
            TempData[_successMessageKey] = "This Email already confirmed";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailVerfication(string userId, bool isProfilePage = false)
        {
            var result = await _accountService.GetVerficationTokenAsync(userId);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction("Register");
            }

            var emailResult = await SendConfirmEmailAsync(_mapper.Map<SendConfirmEmailViewModel>(result.Value));

            if (emailResult.IsSuccess)
            {
                TempData[_successMessageKey] = "Confirmation email resent. Please check your email.";
            }
            else
            {
                TempData[_successMessageKey] = "Failed to resend confirmation email.";
            }
            if (isProfilePage)
            {
                return RedirectToAction("Profile");
            }
            return RedirectToAction("EmailConfirmation", new { userId = result.Value.Id });
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {

            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction("Index", "Home"); // change later to Error Page
            }
            TempData[_successMessageKey] = "Email confirmed successfully.";

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginAsync(_mapper.Map<LoginDto>(model));

                if (result.IsSuccess)
                {
                    TempData[_successMessageKey] = "Login successful.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData[_errorMessageKey] = "Invalid login attempt.";
                }
            }
            return View(model);

        }


        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var result = await _accountService.ForgotPasswordAsync(model.Email);

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return View(model);
            }
            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
               new { token = result.Value.Token, userId = result.Value.Id },
                Request.Scheme
            );

            var emailResult = await _emailService.SendPasswordResetAsync(result.Value.Email, resetLink);
            if (emailResult.IsSuccess)
            {
                TempData[_successMessageKey] =
                            "If an account with that email exists, a password reset link has been sent.";
            }
            else
            {
                TempData[_errorMessageKey] = "Failed to send Reset password email.";
            }
            return View(model);
        }

        public IActionResult ResetPassword(string token, string userId)
        {

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
                return BadRequest("Invalid password reset token.");

            var model = new ResetPasswordViewModel
            {
                Token = token,
                UserId = userId
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.ResetPasswordAsync(_mapper.Map<ResetPasswordDto>(model));
            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return View(model);
            }
            else
            {
                TempData[_successMessageKey] = "Your Password has been reset login again.";
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var result = await _accountService.GetCurrentUserProfileAsync();

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction("Index", "Home");

            }

            var model = _mapper.Map<UpdateProfileViewModel>(result.Value);

            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(UpdateProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.UpdateUserProfileAsync(_mapper.Map<UpdateProfileDto>(model));

                if (result.IsFailure)
                {
                    if (result.Error.Code == "Email")
                    {
                        ModelState.AddModelError("Email", result.Error.Message);
                    }
                    TempData[_errorMessageKey] = result.Error.Message;
                }
                else
                {

                    TempData[_successMessageKey] = "Profile updated successfully!";
                    var profile = _mapper.Map<UpdateProfileViewModel>(result.Value);
                    return View(profile);
                }


            }

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {

            var result = await _accountService.GetCurrentUserProfileAsync();

            if (result.IsFailure)
            {
                TempData[_errorMessageKey] = result.Error.Message;
                return RedirectToAction("Index", "Home");

            }

            var model = new ChangePasswordViewModel
            {
                UserId = result.Value.UserId
            };

            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ChangePasswordAsync(_mapper.Map<ChangePasswordDto>(model));

                if (result.IsSuccess)
                {
                    TempData[_successMessageKey] = "Your password has been changed successfully!";
                }
                else
                {
                    if (result.Error.Code == "CurrentPassword")
                    {

                        ModelState.AddModelError("CurrentPassword", result.Error.Message);
                    }
                    else
                    {
                        TempData[_errorMessageKey] = result.Error.Message;
                    }
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            // delete the shopping cart cookie
            Response.Cookies.Delete("shopping_cart");
            return RedirectToAction("Index", "Home");
        }


        private async Task<Domain.Result.Result> SendConfirmEmailAsync(SendConfirmEmailViewModel model)
        {
            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = model.Id, token = model.Token },
                Request.Scheme
            );


            return await _emailService.SendAsync(
                model.Email!,
                "Confirm your email",
                $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>"
            );
        }
    }
}
