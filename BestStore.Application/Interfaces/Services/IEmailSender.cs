using BestStore.Domain.Result;

namespace BestStore.Application.Interfaces.Services
{
    public interface IEmailSender
    {
        Task<Result> SendAsync(
            string toEmail,
            string subject,
            string htmlContent
        );

        Task<Result> SendPasswordResetAsync(
            string toEmail,
            string resetLink
        );
        Task<Result> SendAsync(string fromEmail, string fromName, string subject, string htmlContent);
    }
}
