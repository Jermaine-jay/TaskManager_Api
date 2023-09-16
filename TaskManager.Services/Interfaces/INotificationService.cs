using TaskManager.Services.Infrastructure;
using Task = TaskManager.Models.Entities.Task;

namespace TaskManager.Services.Interfaces
{
    public interface INotificationService
    {
        Task<object> ToggleNotification(string notiId);
        Task<object> CreateNotification(Task? task, int type);
        Task<SuccessResponse> GetNotifications(string userId);
    }
}
