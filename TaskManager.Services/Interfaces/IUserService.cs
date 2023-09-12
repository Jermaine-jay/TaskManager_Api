using TaskManager.Services.Infrastructure;
using static TaskManager.Services.Implementations.UserService;

namespace TaskManager.Services.Interfaces
{
    public interface IUserService
    {
        Task<SuccessResponse> DeleteUser(string userId);
        Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request);
    }
}
