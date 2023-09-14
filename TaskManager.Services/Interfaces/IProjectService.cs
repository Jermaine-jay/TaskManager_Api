using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;

namespace TaskManager.Services.Interfaces
{
    public interface IProjectService
    {
        Task<SuccessResponse> DeleteProject(string userId, string projectId);
        Task<CreateTaskResponse> CreateProject(string userId, CreateProjectRequest request);
        Task<SuccessResponse> UpdateProject(string userId, UpdateProjectRequest request);
    }
}
