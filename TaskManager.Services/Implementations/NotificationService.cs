using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using Task = TaskManager.Models.Entities.Task;

namespace TaskManager.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Notification> _noteRepo;
        private readonly IRepository<UserTask> _userTaskRepo;
        private readonly IUnitOfWork _unitOfWork;


        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _noteRepo = _unitOfWork.GetRepository<Notification>();
            _userTaskRepo = _unitOfWork.GetRepository<UserTask>();
        }


        public async Task<string> CreateNotification(Task? task, NotificationType type)
        {
            Task? existingtask = await _taskRepo.GetSingleByAsync(t => t.Id.ToString() == task.Id.ToString(), 
                                                    include: u => u.Include(e => e.Project), tracking: true);
            if (existingtask != null)
                throw new InvalidOperationException("Task Not Found");

            IEnumerable<UserTask> userTask = await _userTaskRepo.GetByAsync(u => u.TaskId == task.Id);
            if (userTask == null)
                throw new InvalidOperationException("User Not Found");

            string noteMsg = await Message(type, task);

            foreach (var user in userTask)
            {
                Notification newNote = new Notification
                {
                    Type = type,
                    Message = noteMsg,
                    Timestamp = DateTime.UtcNow,
                    Read = false,
                    UserId = user.UserId
                };
                _noteRepo.Add(newNote);
            }

            await _unitOfWork.SaveChangesAsync();
            return noteMsg;
        }

        public async Task<string> Message(NotificationType type, Task task)
        {
            string reminderMsg = $"This is a reminder that your task {task.Title}, assigned to you on {task.CreatedAt.ToString("dd MMMM yyyy HH: mm:ss")}, is due in 48 hours." +
               $"Please make sure that you have completed all of the necessary steps and that your work is ready to be submitted by the deadline.";

            string StatusMsg = $"Task {task.Title} status changed to {task.Status}";

            string PriorityMsg = $"Task {task.Title} priority changed to {task.Priority}";

            string newTaskAssigned = $"You Have been assigned to a new task {task.Title} with a due date of {task.DueDate}. You are adviced to update your card as you progress";

            string noteMsg = "";
            switch (type)
            {
                case NotificationType.DueDateReminder:
                    noteMsg = reminderMsg;
                    break;

                case NotificationType.StatusUpdate:
                    noteMsg = StatusMsg;
                    break;

                case NotificationType.PriorityUpdate:
                    noteMsg = PriorityMsg;
                    break;
                case NotificationType.NewTaskAssigned:
                    noteMsg = newTaskAssigned;
                    break;
            }

            return noteMsg;
        }

        public async Task<SuccessResponse> ToggleNotification(string notiId)
        {
            Notification notif = await _noteRepo.GetSingleByAsync(u => u.Id.ToString() == notiId);
            if (notif != null)
                throw new InvalidOperationException("Notification Not Found");

            notif.Read = true;
            await _noteRepo.UpdateAsync(notif);

            return new SuccessResponse
            {
                Success = true,
                Data = notif.Read
            };
        }

        public async Task<SuccessResponse> GetNotifications(string userId)
        {
            IEnumerable<Notification> notif = await _noteRepo.GetAllAsync(include: u => u.Include(u => u.User));
            if (notif != null)
                throw new InvalidOperationException("No notification found");

            IEnumerable<Notification> result = notif.Where(u => u.UserId.ToString() == userId);
            if (result == null)
                throw new InvalidOperationException("No notification found");

            return new SuccessResponse
            {
                Success = true,
                Data = result.Select(u => new Notification
                {
                    Type = u.Type,
                    Timestamp = DateTime.Parse(u.Timestamp.ToString("dd MMMM yyyy HH: mm:ss")),
                    Read = u.Read,
                    User = u.User,
                })
            };
        }

        public async Task<bool> CreateReminderNotification()
        {
            IEnumerable<Task> tasks = await _taskRepo.GetAllAsync(include: u => u.Include(u => u.UserTasks));
            if (tasks == null)
                throw new InvalidOperationException("No task Found");

            IEnumerable<Task> results = tasks.Where(u => u.DueDate == u.DueDate.AddHours(-48));
            if (!results.Any())
            {
                foreach (var task in results)
                {
                    await CreateNotification(task, NotificationType.DueDateReminder);
                }
            }

            return false;
        }
    }
}
