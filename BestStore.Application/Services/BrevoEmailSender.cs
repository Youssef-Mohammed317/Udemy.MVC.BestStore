using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.Services
{
    using BestStore.Application.Interfaces.Services;
    using BestStore.Domain.Result;
    using Microsoft.Extensions.Configuration;
    using System.Text;
    using System.Text.Json;

    public class BrevoEmailSender : IEmailSender
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public BrevoEmailSender(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            _httpClient.BaseAddress = new Uri(_config["BrevoSettings:BaseAddress"]);
            _httpClient.DefaultRequestHeaders.Add(
                "api-key",
                _config["BrevoSettings:ApiKey"]
            );
        }

        public async Task<Result> SendAsync(string toEmail, string subject, string htmlContent)
        {
            var body = new
            {
                htmlContent = htmlContent,
                sender = new
                {
                    email = _config["BrevoSettings:SenderEmail"],
                    name = _config["BrevoSettings:SenderName"]
                },
                subject = subject,
                to = new[]
                {
                new
                {
                    email = toEmail
                }
            }
            };
            return await SendAsync(body);

        }

        public async Task<Result> SendAsync(string fromEmail,string fromName, string subject, string htmlContent)
        {
            var body = new
            {
                htmlContent = htmlContent,
                sender = new
                {
                    email = fromEmail,
                    from = fromName
                },
                subject = subject,
                to = new[]
                {
                new
                {
                    email = _config["BrevoSettings:SenderEmail"],
                    name = _config["BrevoSettings:SenderName"]
                }
            }
            };
            return await SendAsync(body);
        }

        private async Task<Result> SendAsync(object body)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync("", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine(error);
                return Result.Failure(Error.Failure("EmailSendingFailed", $"Brevo error:{error}"));
            }
            return Result.Success();
        }

        public Task<Result> SendPasswordResetAsync(string toEmail, string resetLink)
        {
            var html = $@"
            <html>
                <body>
                    <p>Hello,</p>
                    <p>You requested a password reset.</p>
                    <p>
                        <a href='{resetLink}'>Reset your password</a>
                    </p>
                </body>
            </html>";

            return SendAsync(
                toEmail,
                "Reset your password",
                html
            );
        }
    }

}
