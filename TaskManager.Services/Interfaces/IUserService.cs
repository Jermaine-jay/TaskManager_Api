using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;
using static TaskManager.Services.Implementations.UserService;

namespace TaskManager.Services.Interfaces
{
    public interface IUserService
    {
        Task<SuccessResponse> DeleteUser(string userId);
        Task<SuccessResponse> UpdateUser(string userId, UpdateUserRequest request);
        Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request);
    }
}
