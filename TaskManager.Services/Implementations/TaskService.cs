using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
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
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;


        public TaskService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _serviceProvider = serviceProvider;
        }

        public async Task<SuccessResponse> CreateTask(string userId, CreateTaskRequest request)
        {
            var user = await _userRepo.GetSingleByAsync(u => u.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            var project = await _projectRepo.GetSingleByAsync(p => p.Id.ToString() == request.ProjectId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            if (project.UserId != user.Id)
                throw new InvalidOperationException("You cannot add task to this project");

            var existingTile = project.Tasks.Any(u => u.Title == request.Title);
            if (existingTile)
                throw new InvalidOperationException("Task Title already exist");

            var priority = Priority.Low;
            switch (request.Priority)
            {
                case (int)Priority.Low:
                    priority = Priority.Low;
                    break;

                case (int)Priority.Medium:
                    priority = Priority.Medium;
                    break;

                case (int)Priority.High:
                    priority = Priority.High;
                    break;
            }

            var newTask = new Task
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = DateTime.Parse(request.DueDate),
                Priority = priority,
                Status = Status.Pending,
                ProjectId = project.Id,
            };

            await _taskRepo.AddAsync(newTask);
            return new SuccessResponse
            {
                Success = true,
                Data = newTask
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
                UpdatedAt = DateTime.UtcNow
            };

            await _taskRepo.UpdateAsync(newTask);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<UpdateTaskResponse> UpdateStatus(string userId, UpdateStatusRequest request)
        {
            var project = await _projectRepo.GetSingleByAsync(user => user.UserId.ToString() == userId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            var task = project.Tasks.Where(u => u.Id.ToString() == request.TaskId).FirstOrDefault();
            if (task == null)
                throw new InvalidOperationException("User does not exist");


            switch (request.Status)
            {
                case (int)Status.InProgress:
                    task.Status = Status.InProgress;
                    task.UpdatedAt = DateTime.UtcNow;
                    await _taskRepo.UpdateAsync(task);
                    break;

                case (int)Status.Pending:
                    task.Status = Status.Pending;
                    task.UpdatedAt = DateTime.UtcNow;
                    await _taskRepo.UpdateAsync(task);
                    break;

                case (int)Status.Completed:
                    task.Status = Status.Completed;
                    task.UpdatedAt = DateTime.UtcNow;
                    await _taskRepo.UpdateAsync(task);
                    break;
            }

            await _serviceProvider.GetService<INotificationService>().CreateNotification(task, (int)NotificationType.StatusUpdate);
            return new UpdateTaskResponse
            {
                Message = "Status Updated",
                Status = HttpStatusCode.OK,
                Success = true,
                Data = task,
            };
        }

        public async Task<UpdateTaskResponse> UpdatePriority(string userId, UpdatePriorityRequest request)
        {
            var project = await _projectRepo.GetSingleByAsync(user => user.UserId.ToString() == userId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            var task = project.Tasks.Where(u => u.Id.ToString() == request.TaskId).FirstOrDefault();
            if (task == null)
                throw new InvalidOperationException("User does not exist");

            switch (request.Priority)
            {
                case (int)Priority.Low:
                    task.Priority = Priority.Low;
                    task.UpdatedAt = DateTime.UtcNow;
                    await _taskRepo.UpdateAsync(task);
                    break;

                case (int)Priority.Medium:
                    task.Priority = Priority.Medium;
                    task.UpdatedAt = DateTime.UtcNow;
                    await _taskRepo.UpdateAsync(task);
                    break;

                case (int)Priority.High:
                    task.Status = Status.Completed;
                    task.UpdatedAt = DateTime.UtcNow;
                    await _taskRepo.UpdateAsync(task);
                    break;
            }

            await _serviceProvider.GetService<INotificationService>().CreateNotification(task, (int)NotificationType.PriorityUpdate);
            return new UpdateTaskResponse
            {
                Message = "Priority Updated",
                Status = HttpStatusCode.OK,
                Success = true,
                Data = task,
            };
        }

       
    }
}
