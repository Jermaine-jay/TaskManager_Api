using Microsoft.AspNetCore.Identity;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementations
{
    public class AdminService : IAdminService
    {

        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
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
    }
}
