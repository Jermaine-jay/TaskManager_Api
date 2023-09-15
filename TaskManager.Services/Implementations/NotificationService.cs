using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
        private readonly IServiceFactory _serviceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<Notification> _noteRepo;
        private readonly IRepository<UserTask> _userTaskRepo;
        private readonly IUnitOfWork _unitOfWork;


        public NotificationService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
            _userManager = userManager;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _noteRepo = _unitOfWork.GetRepository<Notification>();
            _userTaskRepo = _unitOfWork.GetRepository<UserTask>();
        }


        public async Task<object> CreateNotification(Task? task, int type)
        {

            var userTask = await _userTaskRepo.GetByAsync(u => u.TaskId.Equals(task.Id));
            if (userTask == null)
                throw new InvalidOperationException("User Not Found");


            var existingtask = await _taskRepo.GetSingleByAsync(t => t.Id.ToString() == task.Id.ToString(), include: u => u.Include(e => e.Project), tracking:true);
            if (existingtask != null)       
                throw new InvalidOperationException("Task Not Found");


            var noteMsg = await Message(type, task);
            var newNote = new Notification
            {
                Type = NotificationType.DueDateReminder,
                Message = noteMsg,
                Timestamp = DateTime.Now,
                Read = false,
            };

            foreach(var user in userTask)
            {
                var result =await _userRepo.GetSingleByAsync(u=> u.Id.Equals(user.UserId), include: u=> u.Include(e => e.Notifications));
                result.Notifications.Add(newNote);
            }

            await _noteRepo.AddAsync(newNote);
            return newNote;
        }


        public async Task<string> Message(int type, Task task)
        {
            string reminderMsg = $"This is a reminder that your task {task.Title}, assigned to you on {task.CreatedAt.ToString("dd MM YY")}, is due in 48 hours." +
               $"Please make sure that you have completed all of the necessary steps and that your work is ready to be submitted by the deadline.";

            string StatusMsg = $"Task {task.Title} status changed to {task.Status}";

            string PriorityMsg = $"Task {task.Title} priority changed to {task.Priority}";

            var noteMsg = "";
            switch (type)
            {
                case (int)NotificationType.DueDateReminder:
                    noteMsg = reminderMsg;
                    return null;

                case (int)NotificationType.StatusUpdate:
                    noteMsg = StatusMsg;
                    return null;

                case (int)NotificationType.PriorityUpdate:
                    noteMsg = PriorityMsg;
                    return null;
            }

            return noteMsg;
        }


        public async Task<object> ToggleNotification(string notiId)
        {
            var notif = await _noteRepo.GetSingleByAsync(u => u.Id.ToString() == notiId);
            if (notif != null) 
                throw new InvalidOperationException("Notification Not Found");

            notif.Read = true;
            _noteRepo.UpdateAsync(notif);

            return new SuccessResponse
            {
                Success = true,
                Data = notif.Read
            };
        }

        public async Task<SuccessResponse> GetNotifications(string userId)
        {
            var notif = await _noteRepo.GetAllAsync(include: u => u.Include(u => u.User));
            var result = notif.Where(u => u.UserId.ToString() == userId);
            if (result == null)
                throw new InvalidOperationException("No notification found");

            return new SuccessResponse
            {
                Success = true,
                Data = result
            };
        }
    }
}
