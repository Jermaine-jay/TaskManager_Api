using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using Task = TaskManager.Models.Entities.Task;

namespace TaskManager.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IUnitOfWork _unitOfWork;


        public UserService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
            _userManager = userManager;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }


        public async Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Invalid User");


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
                throw new InvalidOperationException("User does not exist");

            await _userRepo.DeleteAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<Object> CreateTask(string userId, CreateTaskRequest request)
        {
            var user = await _userRepo.GetSingleByAsync(u => u.Id.ToString() == userId, include: u=> u.Include(u=>u.Tasks));
            if (user == null)
                throw new InvalidOperationException("Invalid user");
            var project =  await _projectRepo.GetSingleByAsync(p=> p.Equals(user));
            throw new InvalidOperationException("Invalid username or password");
        }

        public class ChangePasswordRequest
        {

            [Required, DataType(DataType.Password)]
            public string CurrentPassword { get; set; }

            [Required, DataType(DataType.Password)]
            public string NewPassword { get; set; }
        }

        public class CreateTaskRequest
        {
            [Required]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            public string? DueDate { get; set; }

            public string? Priority { get; set; }

            public string? Status { get; set; } 

            public 
        }
    }
}
