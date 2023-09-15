using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Implementations;

namespace TaskManager.Services.Interfaces
{
    public interface IUserService
    {
        Task<SuccessResponse> DeleteUser(string userId);
        Task<SuccessResponse> UpdateUser(string userId, UpdateUserRequest request);
        Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request);
        Task<SuccessResponse> GetAllTask(string userId);
        Task<SuccessResponse> GetAllProject(string userId);
        Task<SuccessResponse> AddUserToTask(UserTaskRequest request);
        Task<SuccessResponse> AllProjectWithTask(string userId);
    }
}
