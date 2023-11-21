using TaskManager.Services.Infrastructure;
using TaskManager.Services.Implementations;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;

namespace TaskManager.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllRoles();
        Task<IEnumerable<string>> GetUserRoles(string userName);
        Task<SuccessResponse> RemoveUserFromRole(AddUserToRoleRequest request);
        Task<SuccessResponse> EditRole(string id, string Name);
        Task<SuccessResponse> DeleteRole(string name);
        Task<AddUserToRoleResponse> AddUserToRole(AddUserToRoleRequest request);
        Task<SuccessResponse> CreateRoleAync(RoleDto request);
    }
}
