using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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

    }
}
