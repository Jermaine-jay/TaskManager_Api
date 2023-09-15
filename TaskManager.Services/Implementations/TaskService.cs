using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using Task = TaskManager.Models.Entities.Task;

namespace TaskManager.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;


        public TaskService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
            _userManager = userManager;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _notificationService = notificationService;
        }


        public async Task<CreateTaskResponse> CreateTask(string userId, CreateTaskRequest request)
        {
            var user = await _userRepo.GetSingleByAsync(u => u.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            var project = await _projectRepo.GetSingleByAsync(p => p.Equals(request.ProjectId));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            if (project.UserId != user.Id)
                throw new InvalidOperationException("Youn cannot add task to this project");

            var existingTile = project.Tasks.Any(u => u.Title == request.Title);
            if (existingTile)
                throw new InvalidOperationException("Task Title already exist");

            var priority = Priority.Low;
            switch (request.Priority)
            {
                case (int)Priority.Low:
                    priority = Priority.Low;
                    return null;

                case (int)Priority.Medium:
                    priority = Priority.Medium;
                    return null;

                case (int)Priority.High:
                    priority = Priority.High;
                    return null;
            }

            var newTask = new Task
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                DueDate = DateTime.Parse(request.DueDate),
                Priority = priority,
                ProjectId = project.Id,
            };

            await _taskRepo.AddAsync(newTask);
            return new CreateTaskResponse
            {
                Message = "Task Created",
                Status = HttpStatusCode.Created,
                Success = true,
                Data = newTask,
            };
        }


        public async Task<SuccessResponse> DeleteTask(string userId, string taskId)
        {
            var project = await _projectRepo.GetSingleByAsync(user => user.UserId.ToString() == userId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            var task = project.Tasks.Where(u => u.Id.ToString() == taskId).FirstOrDefault();
            if (task == null)
                throw new InvalidOperationException("Task does not exist");

            await _taskRepo.DeleteAsync(task);
            return new SuccessResponse
            {
                Success = true
            };
        }


        public async Task<SuccessResponse> UpdateTask(string userId, UpdateTaskRequest request)
        {
            var project = await _projectRepo.GetSingleByAsync(user => user.UserId.ToString() == userId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            var task = project.Tasks.Where(u => u.Id.ToString() == request.TaskId).FirstOrDefault();
            if (task == null)
                throw new InvalidOperationException("User does not exist");

            var newTask = new Task
            {
                Title = request.Title,
                DueDate = DateTime.Parse(request.DueDate),
                Description = request.Description,
                UpdatedAt = DateTime.Now
            };

            await _taskRepo.UpdateAsync(newTask);
            return new SuccessResponse
            {
                Success = true
            };
        }


        public async Task<UpdateTaskResponse> UpdateStatus(UpdateStatusRequest request)
        {
            var task = await _taskRepo.GetSingleByAsync(c => c.Id.ToString() == request.taskId);
            if (task == null)
                throw new InvalidOperationException("Task does not exist");


            switch (request.Status)
            {
                case (int)Status.InProgress:
                    task.Status = Status.InProgress;
                    await _taskRepo.UpdateAsync(task);
                    return null;

                case (int)Status.Pending:
                    task.Status = Status.Pending;
                    await _taskRepo.UpdateAsync(task);
                    return null;

                case (int)Status.Completed:
                    task.Status = Status.Completed;
                    await _taskRepo.UpdateAsync(task);

                    break;
            }

            await _notificationService.CreateNotification(task, (int)NotificationType.StatusUpdate);
            return new UpdateTaskResponse
            {
                Message = "Status Updated",
                Status = HttpStatusCode.OK,
                Success = true,
                Data = task,
            };
        }


        public async Task<UpdateTaskResponse> UpdatePriority(UpdatePriorityRequest request)
        {
            var task = await _taskRepo.GetSingleByAsync(c => c.Id.ToString() == request.taskId);
            if (task == null)
                throw new InvalidOperationException("Task does not exist");


            switch (request.Priority)
            {
                case (int)Priority.Low:
                    task.Priority = Priority.Low;
                    await _taskRepo.UpdateAsync(task);
                    return null;

                case (int)Priority.Medium:
                    task.Priority = Priority.Medium;
                    await _taskRepo.UpdateAsync(task);
                    return null;

                case (int)Priority.High:
                    task.Status = Status.Completed;
                    await _taskRepo.UpdateAsync(task);

                    break;
            }

            await _notificationService.CreateNotification(task, (int)NotificationType.PriorityUpdate);
            return new UpdateTaskResponse
            {
                Message = "Priority Updated",
                Status = HttpStatusCode.OK,
                Success = true,
                Data = task,
            };
        }


        public async System.Threading.Tasks.Task AllTask()
        {
            var tasks = await _taskRepo.GetAllAsync(include: u=> u.Include(u=> u.UserTasks));
            if(tasks == null)
                throw new InvalidOperationException("No task Found");

            var results = tasks.Where(u=> u.DueDate == u.DueDate.AddHours(-48));
            foreach(var task in results)
            {
                await _notificationService.CreateNotification(task, (int)NotificationType.DueDateReminder);
            }
        }

    }
}
