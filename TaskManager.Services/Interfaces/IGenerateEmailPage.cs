using TaskManager.Models.Enums;

namespace TaskManager.Services.Interfaces
{
    public interface IGenerateEmailPage
    {
        public string EmailVerificationPage(string name, string token);

        string PasswordResetPage(string callbackurl);
        string TaskNotificationPage(string message, string callbackUrl, NotificationType notificationType);
    }
}
