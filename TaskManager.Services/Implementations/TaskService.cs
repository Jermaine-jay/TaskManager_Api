using System.Net;
using TaskManager.Models.Enums;
using TaskManager.Models.Entities;
using TaskManager.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskManager.Services.Interfaces;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;
using Task = TaskManager.Models.Entities.Task;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;


namespace TaskManager.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Task> _taskRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<ApplicationUser> _userRepo;


        public TaskService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }

        public async Task<SuccessResponse> CreateTask(string userId, CreateTaskRequest request)
        {
            ApplicationUser user = await _userRepo.GetSingleByAsync(u 
                => u.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            Project project = await _projectRepo.GetSingleByAsync(p 
                => p.Id.ToString() == request.ProjectId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            if (project.UserId != user.Id)
                throw new InvalidOperationException("You cannot add task to this project");

            bool existingTile = project.Tasks.Any(u => u.Title == request.Title);
            if (existingTile)
                throw new InvalidOperationException("Task Title already exist");

            Priority priority = Priority.Low;
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

            Task newTask = new Task
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = DateTime.Parse(request.DueDate),
                Priority = priority,
                Status = Status.Pending,
                ProjectId = project.Id,
            };

            _taskRepo.Add(newTask);
            await _serviceProvider.GetService<INotificationService>()
                .CreateNotification(newTask, NotificationType.NewTaskAssigned);

            await _unitOfWork.SaveChangesAsync();
            return new SuccessResponse
            {
                Success = true,
                Data = newTask
            };
        }

        public async Task<SuccessResponse> GetTask(string taskId, string userId)
        {
            Task? task = await _taskRepo.GetSingleByAsync(x 
                => x.Id.ToString() == taskId, include: x 
                => x.Include(x => x.UserTasks).Include(x => x.Project))
                ?? throw new InvalidDataException("Task does not exist");

            ApplicationUser user = await _userRepo.GetSingleByAsync(u 
                => u.Id.ToString() == userId)
                ?? throw new InvalidOperationException("User Not Found");

            if (task.Project.UserId.ToString() == userId 
                || task.UserTasks.Any(x => x.UserId.ToString() == userId))
            {
                TaskResponse response = new()
                { 
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate.ToString(),
                    Priority = task.Priority.ToString(),
                    Status = task.Status.ToString(),
                };

                return new SuccessResponse { Success = true, Data = response };
            }

            return new SuccessResponse { Success = false};
        }

        public async Task<bool> AssignTask(string taskId, List<string> usersid)
        {
            Task? task = await _taskRepo.GetSingleByAsync(x 
                => x.Id.ToString() == taskId, include: x 
                => x.Include(x => x.UserTasks).Include(x => x.Project))
                ?? throw new InvalidDataException("Task Does Not Exist");

            foreach (string userid in usersid)
            {
                var user = await _userRepo.GetSingleByAsync(x => x.Id.ToString() == userid);
                if (!task.UserTasks.Any(x => x.UserId.ToString() == userid))
                {
                    UserTask userTask = new()
                    {
                        User = user,
                        Task = task,
                    };
                    task.UserTasks.Add(userTask);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<SuccessResponse> DeleteTask(string userId, string taskId)
        {
            Project? project = await _projectRepo.GetSingleByAsync(user 
                => user.UserId.ToString() == userId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            Task? task = project.Tasks?.Where(u => u.Id.ToString() == taskId).SingleOrDefault();
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
            Project? project = await _projectRepo.GetSingleByAsync(user 
                => user.UserId.ToString() == userId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            Task? task = project.Tasks?.Where(u => u.Id.ToString() == request.TaskId).SingleOrDefault();
            if (task == null)
                throw new InvalidOperationException("User does not exist");

            Task newTask = new Task
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
            Project? project = await _projectRepo.GetSingleByAsync(user 
                => user.UserId.ToString() == userId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            Task? task = project.Tasks.Where(u 
                => u.Id.ToString() == request.TaskId).SingleOrDefault();
            if (task == null)
                throw new InvalidOperationException("User does not exist");

            switch (request.Status)
            {
                case (int)Status.InProgress:
                    task.Status = Status.InProgress;
                    task.UpdatedAt = DateTime.UtcNow;
                    _taskRepo.Update(task);
                    break;

                case (int)Status.Pending:
                    task.Status = Status.Pending;
                    task.UpdatedAt = DateTime.UtcNow;
                    _taskRepo.Update(task);
                    break;

                case (int)Status.Completed:
                    task.Status = Status.Completed;
                    task.UpdatedAt = DateTime.UtcNow;
                    _taskRepo.Update(task);
                    break;
            }

            await _serviceProvider.GetService<INotificationService>()
                    .CreateNotification(task, NotificationType.StatusUpdate);

            await _unitOfWork.SaveChangesAsync();

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
            Project? project = await _projectRepo.GetSingleByAsync(user 
                => user.UserId.ToString() == userId, include: u => u.Include(u => u.Tasks));
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            Task? task = project.Tasks?.Where(u => u.Id.ToString() == request.TaskId).SingleOrDefault();
            if (task == null)
                throw new InvalidOperationException("User does not exist");

            switch (request.Priority)
            {
                case (int)Priority.Low:
                    task.Priority = Priority.Low;
                    task.UpdatedAt = DateTime.UtcNow;
                    _taskRepo.Update(task);
                    break;

                case (int)Priority.Medium:
                    task.Priority = Priority.Medium;
                    task.UpdatedAt = DateTime.UtcNow;
                    _taskRepo.Update(task);
                    break;

                case (int)Priority.High:
                    task.Status = Status.Completed;
                    task.UpdatedAt = DateTime.UtcNow;
                    _taskRepo.Update(task);
                    break;
            }

            await _serviceProvider.GetService<INotificationService>().CreateNotification(task, NotificationType.PriorityUpdate);

            await _unitOfWork.SaveChangesAsync();

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
