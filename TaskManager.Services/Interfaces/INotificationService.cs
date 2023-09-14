using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Services.Interfaces
{
    public interface INotificationService
    {
        Task<object> ToggleNotification(string notiId);
        Task<object> CreateNotification(Task? task, int type);
    }
}
