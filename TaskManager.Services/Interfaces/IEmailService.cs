using TaskManager.Models.Entities;
using TaskManager.Models.Enums;

namespace TaskManager.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> VerifyEmailAddress(string emailAddress);
        Task<bool> RegistrationMail(ApplicationUser user);
        Task<bool> Execute(string email, string subject, string htmlMessage);
        Task<object> SendEmailAsync(string subject, string message, string email);
        Task<string> ResetPasswordMail(ApplicationUser user);
        Task<bool> TaskMail(string taskId, string email, string message, NotificationType notification);
    }
}
