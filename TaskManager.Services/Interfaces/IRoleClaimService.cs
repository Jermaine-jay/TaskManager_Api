using TaskManager.Services.Implementations;
using TaskManager.Services.Infrastructure;

namespace TaskManager.Services.Interfaces
{
    public interface IRoleClaimService
    {
        Task<RoleClaimResponse> UpdateRoleClaims(UpdateRoleClaimsDto request);
        Task<ServiceResponse> RemoveUserClaims(string claimType, string role);
        Task<SuccessResponse> GetUserClaims(string? role);
        Task<ServiceResponse<RoleClaimResponse>> AddClaim(RoleClaimRequest request);
    }
}
