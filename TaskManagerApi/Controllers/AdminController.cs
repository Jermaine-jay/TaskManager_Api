using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Models.Dtos;
using TaskManager.Models.Dtos.Request;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;


namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Authorization")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProjectService _projectService;
        public AdminController(IAdminService adminService, IProjectService projectService)
        {
            _adminService = adminService;
            _projectService = projectService;
        }

        [HttpGet("all-users", Name = "all-users")]
        [SwaggerOperation(Summary = "Get All Registered Users")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Users", Type = typeof(ApplicationUserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _adminService.GetUsers();
            return Ok(response);
        }


        [HttpDelete("remove-user", Name = "remove-user")]
        [SwaggerOperation(Summary = "Delete a user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var response = await _adminService.DeleteUser(userId);
            return Ok(response);

        }


        [HttpGet("get-a-user", Name = "get-a-user")]
        [SwaggerOperation(Summary = "Get A Registered User")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "successful", Type = typeof(ApplicationUserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUser(string userId)
        {
            var response = await _adminService.GetUser(userId);
            return Ok(response);
        }


        [HttpPost("lock-user", Name = "lock-user")]
        [SwaggerOperation(Summary = "Block A Registered User")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> LockUser([FromBody] LockUserRequest request)
        {
            var response = await _adminService.LockUser(request);
            return Ok(response);
        }


        [HttpGet("get-user-projects", Name = "get-user-projects")]
        [SwaggerOperation(Summary = "get user projects and tasks")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "user projects and tasks", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "No projects found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UserProjectsWithTasks(string userId)
        {
            var response = await _adminService.UserProjectsWithTasks(userId);
            return Ok(response);
        }


        [HttpGet("all-users-projects", Name = "all-users-projects")]
        [SwaggerOperation(Summary = "all users projects")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "users projects with tasks", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "No project found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "No user found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AllUsersProjectsWithTasks()
        {
            var response = await _adminService.AllUsersProjectsWithTasks();
            return Ok(response);
        }


        [HttpDelete("delete-user-project", Name = "delete-user-project")]
        [SwaggerOperation(Summary = "Delete a user project")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "true or false", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Project Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteProject([FromQuery] string userId, string projectId)
        {
            var response = await _projectService.DeleteProject(userId, projectId);
            return Ok(response);
        }
    }
}
