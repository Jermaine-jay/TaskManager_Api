using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Entities;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using Task = TaskManager.Models.Entities.Task;


namespace TaskManager.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly INotificationService _notificationService;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<UserTask> _userTaskRepo;
        private readonly IUnitOfWork _unitOfWork;


        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork; 
            _userManager = userManager;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _userTaskRepo = _unitOfWork.GetRepository<UserTask>();
           // _notificationService = notificationService;
        }


        public async Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");


            await _userManager.ChangePasswordAsync(user, request.NewPassword, request.CurrentPassword);
            return new SuccessResponse
            {
                Success = true,
            };
        }

        public async Task<SuccessResponse> DeleteUser(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            await _userRepo.DeleteAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<ApplicationUserDto> GetUser(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            return new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Active = user.Active ? "Active" : "Not Active",
                EmailConfirmed = user.EmailConfirmed ? "Confirmed" : "Not Confirmed",
                LockedOut = user.LockoutEnd?.ToString("dd MMMM yyyy HH:mm:ss"),
                CreatedAt = user.CreatedAt.ToString("dd MMMM yyyy HH:mm:ss"),
                UpdatedAt = user.UpdatedAt.ToString("dd MMMM yyyy HH:mm:ss"),
            };
        }

        public async Task<SuccessResponse> UpdateUser(string userId, UpdateUserRequest request)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _userManager.UpdateAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }


        public async Task<SuccessResponse> GetAllTask(string userId)
        {
            var userTask = await _userTaskRepo.GetAllAsync(include: u => u.Include(e => e.User));
            if (userTask == null)
                throw new InvalidOperationException("User Does Not Exist");


            var result = userTask.Where(u => u.UserId.ToString() == userId).Select(u => new Task
            {
                Title = u.Task.Title,
                Description = u.Task.Description,
                DueDate = DateTime.Parse(u.Task.DueDate.ToString("dd MM YY")),
                Priority = u.Task.Priority,
                Status = u.Task.Status,
            });

            if (!result.Any())
                throw new InvalidOperationException("No task assigned to you");

            return new SuccessResponse
            {
                Success = true,
                Data = result
            };
        }


        public async Task<SuccessResponse> GetAllProject(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            var projects = await _projectRepo.GetAllAsync(include: u => u.Include(u => u.Tasks));
            if (!projects.Any())
                throw new InvalidOperationException("No project found");

            var result = projects.Where(u => u.UserId.ToString() == user.Id.ToString()).ToList();
            if (!result.Any())
                throw new InvalidOperationException("User Projects Not Found");

            var response = result.Select(u => new Project
            {
                Name = u.Name,
                Description = u.Description,
            });

            return new SuccessResponse
            {
                Success = true,
                Data = response
            };
        }


        public async Task<SuccessResponse> AllProjectWithTask(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            var projects = await _projectRepo.GetAllAsync(include: u => u.Include(u => u.Tasks));
            if (!projects.Any())
                throw new InvalidOperationException("No project found");

            var result = projects.Where(u => u.UserId.ToString() == user.Id.ToString());
            if (!result.Any())
                throw new InvalidOperationException("User Not Found");

            var res = result.Select(u => new Project
            {
                Name = u.Name,
                Description = u.Description,
                Tasks = u.Tasks.Select(u => new Task
                {
                    Title = u.Title,
                    Description = u.Description,
                    Priority = u.Priority,
                    DueDate = DateTime.Parse(u.DueDate.ToString("dd MMMM yyyy HH:mm:ss")),
                    Status = u.Status,
                }).ToList()
            });

            return new SuccessResponse
            {
                Success = true,
                Data = res
            };
        }


        public async Task<SuccessResponse> AddUserToTask(string userId, UserTaskRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            var task = await _taskRepo.GetSingleByAsync(u => u.Id.ToString() == request.TaskId);
            if (task == null)
                throw new InvalidOperationException("Task does not exist");

            var proj = await _projectRepo.GetAllAsync(include: u => u.Include(u => u.Tasks));
            var project = proj.Where(u => u.Tasks.Any(u => u.Id == task.Id)).FirstOrDefault();
            if (project.UserId.ToString() != userId)
                throw new InvalidOperationException("You cannot perform this operation");

            var newUserTask = new UserTask
            {
                TaskId = task.Id,
                User = user,
            };

            await _userTaskRepo.AddAsync(newUserTask);
            return new SuccessResponse
            {
                Success = true
            };

        }

    }

}
