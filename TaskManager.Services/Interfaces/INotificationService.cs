using TaskManager.Models.Enums;
using TaskManager.Services.Infrastructure;
using Task = TaskManager.Models.Entities.Task;

namespace TaskManager.Services.Interfaces
{
    public interface INotificationService
    {
        Task<SuccessResponse> ToggleNotification(string notiId);
        Task<string> CreateNotification(Task? task, NotificationType type);
        Task<SuccessResponse> GetNotifications(string userId);
        Task<bool> CreateReminderNotification();
        Task<bool> CreateNewTaskNotification();
    }
}
