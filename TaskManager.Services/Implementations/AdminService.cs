using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using Task = TaskManager.Models.Entities.Task;

namespace TaskManager.Services.Implementations
{
    public class AdminService : IAdminService
    {

        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
        }


        public async Task<IEnumerable <ApplicationUserDto>> GetUsers()
        {
            var users = await _userRepo.GetAllAsync();
            if (users == null)
                throw new InvalidOperationException("Users Not Found");

            return users.Select(user => new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Active = user.Active ? "Active" : "Not Active",
                EmailConfirmed = user.EmailConfirmed ? "Confirmed" : "Not Confirmed",
                LockedOut = user.LockoutEnd?.ToString("dd MMMM yyyy"),
                CreatedAt = user.CreatedAt.ToString("dd MMMM yyyy"),
                UpdatedAt = user.UpdatedAt.ToString("dd MMMM yyyy"),
            });         
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
               Active = user.Active? "Active" : "Not Active",
               EmailConfirmed = user.EmailConfirmed? "Confirmed": "Not Confirmed",
               LockedOut = user.LockoutEnd?.ToString("dd MMMM yyyy HH:mm:ss"),
               CreatedAt = user.CreatedAt.ToString("dd MMMM yyyy HH:mm:ss"),
                UpdatedAt = user.UpdatedAt.ToString("dd MMMM yyyy HH:mm:ss"),              
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


        public async Task<SuccessResponse> LockUser(LockUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            DateTimeOffset lockoutEnd = DateTimeOffset.UtcNow.AddHours(request.Duration);
            user.LockoutEnd = lockoutEnd;

            await _userManager.UpdateAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }


        public async Task<SuccessResponse> UsersProjectsWithTasks()
        {
            var projects = await _projectRepo.GetAllAsync(include: u => u.Include(u => u.Tasks));
            if (!projects.Any())
                throw new InvalidOperationException("No project found");

            var result = projects.Select(u => new Project
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
                Data = result
            };
        }


        public async Task<SuccessResponse> UserProjectsWithTasks(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
                throw new InvalidOperationException("User Not Found");

            var projects = await _projectRepo.GetAllAsync(include: u => u.Include(u => u.Tasks));
            if (!projects.Any())
                throw new InvalidOperationException("No project found");

            var result = projects.Where(u => u.UserId.ToString() == user.Id.ToString()).ToList();
            if(!result.Any())
                throw new InvalidOperationException("User Projects Not Found");

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
    }
}
