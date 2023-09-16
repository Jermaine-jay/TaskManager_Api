using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;

namespace TaskManager.Services.Interfaces
{
    public interface ITaskService
    {
        Task<SuccessResponse> CreateTask(string userId, CreateTaskRequest request);
        Task<SuccessResponse> UpdateTask(string userId, UpdateTaskRequest request);
        Task<UpdateTaskResponse> UpdatePriority(string userId, UpdatePriorityRequest request);
        Task<UpdateTaskResponse> UpdateStatus(string userId, UpdateStatusRequest request);
        Task<SuccessResponse> DeleteTask(string taskId, string userId);
        Task<bool> AllTask();
    }
}
