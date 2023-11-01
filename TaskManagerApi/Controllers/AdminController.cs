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
    //[Authorize(Policy = "Authorization")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        [AllowAnonymous]
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



        [HttpDelete("remove-user", Name = "remove-User")]
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


        [HttpGet("all-user-projects", Name = "all-user-projects")]
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
        public async Task<IActionResult> UsersProjectsWithTasks()
        {
            var response = await _adminService.UsersProjectsWithTasks();
            return Ok(response);
        }

    }
}
