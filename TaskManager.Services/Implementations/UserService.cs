using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
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
        }

        public async Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
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
            ApplicationUser user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            await _userRepo.DeleteAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<ProfileResponse> GetUser(string userId)
        {
            ApplicationUser user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            ProfileResponse result = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Active = user.Active ? "Active" : "Not Active",
                EmailConfirmed = user.EmailConfirmed ? "Confirmed" : "Not Confirmed",
            };

            return result;
        }

        public async Task<SuccessResponse> UpdateUser(string userId, UpdateUserRequest request)
        {
            ApplicationUser user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
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
            IEnumerable<UserTask> userTask = await _userTaskRepo.GetAllAsync(include: u => u.Include(e => e.User));
            if (userTask == null)
                throw new InvalidOperationException("User Does Not Exist");


            IEnumerable<Task> result = userTask.Where(u => u.UserId.ToString() == userId).Select(u => new Task
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
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            IEnumerable<Project> projects = await _projectRepo.GetAllAsync(include: u => u.Include(u => u.Tasks));
            if (!projects.Any())
                throw new InvalidOperationException("No project found");

            List<Project> result = projects.Where(u => u.UserId.ToString() == user.Id.ToString()).ToList();
            if (!result.Any())
                throw new InvalidOperationException("User Projects Not Found");

            IEnumerable<Project> response = result.Select(u => new Project
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
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            IEnumerable<Project> projects = await _projectRepo.GetAllAsync(include: u => u.Include(u => u.Tasks));
            if (!projects.Any())
                throw new InvalidOperationException("No project found");

            IEnumerable<Project> result = projects.Where(u => u.UserId.ToString() == user.Id.ToString());
            if (!result.Any())
                throw new InvalidOperationException("User Not Found");

            IEnumerable<Project> res = result.Select(u => new Project
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
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            Task? task = await _taskRepo.GetSingleByAsync(u => u.Id.ToString() == request.TaskId);
            if (task == null)
                throw new InvalidOperationException("Task does not exist");

            IEnumerable<Project> proj = await _projectRepo.GetAllAsync(include: u => u.Include(u => u.Tasks));

            Project? project = proj.Where(u => u.Tasks.Any(u => u.Id == task.Id)).SingleOrDefault();
            if (project.UserId.ToString() != userId)
                throw new InvalidOperationException("You cannot perform this operation");

            foreach (string userid in request.UsersId)
            {
                ApplicationUser? getUser = await _userRepo.GetSingleByAsync(x => x.Id.ToString() == userid);
                if (!task.UserTasks.Any(x => x.UserId.ToString() == userid))
                {
                    UserTask userTask = new()
                    {
                        User = getUser,
                        Task = task,
                    };
                    task.UserTasks.Add(userTask);
                }
            }
            
            await _unitOfWork.SaveChangesAsync();
            return new SuccessResponse
            {
                Success = true
            };

        }


        public async Task<SuccessResponse> PickTask(string userId, string taskId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            Task? task = await _taskRepo.GetSingleByAsync(u => u.Id.ToString() == taskId, include: u => u.Include(u => u.UserTasks));
            if (task == null)
                throw new InvalidOperationException("Task does not exist");

            UserTask? existinguser = task.UserTasks.Where(u => u.UserId == user.Id).SingleOrDefault()
                ?? throw new InvalidOperationException("User already has this task");

            UserTask newUserTask = new()
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
