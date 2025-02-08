using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Api.Extensions;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;


namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Authorization")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly INotificationService _noteService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpContextAccessor contextAccessor, IUserService userService, INotificationService noteService)
        {
            _httpContextAccessor = contextAccessor;
            _userService = userService;
            _noteService = noteService;
        }


        [HttpGet("my-account", Name = "my-account")]
        [SwaggerOperation(Summary = "get loggedin user account ")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "user", Type = typeof(ProfileResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUser()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            ProfileResponse response = await _userService.GetUser(userId);
            return Ok(response);
        }


        [HttpPut("change-password", Name = "change-password")]
        [SwaggerOperation(Summary = "Change user password")]
        [SwaggerResponse(StatusCodes.Status202Accepted, Description = "User", Type = typeof(ChangePasswordResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _userService.ChangePassword(userId, request);
            return Ok(response);
        }


        [HttpDelete("delete-user", Name = "delete-user")]
        [SwaggerOperation(Summary = "delete a user")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteUser()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _userService.DeleteUser(userId);
            return Ok(response);
        }


        [HttpPut("update-user", Name = "update-user")]
        [SwaggerOperation(Summary = "update a user")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _userService.UpdateUser(userId, request);
            return Ok(response);
        }


        [HttpGet("user-tasks", Name = "user-tasks")]
        [SwaggerOperation(Summary = "all user tasks")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user task", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "No task assigned to you", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetAllTask()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _userService.GetAllTask(userId);
            return Ok(response);
        }


        [HttpGet("user-projects", Name = "user-projects")]
        [SwaggerOperation(Summary = "all user projects")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user projects", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetAllProjects()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _userService.GetAllProject(userId);
            return Ok(response);
        }


        [HttpGet("project-task", Name = "project-task")]
        [SwaggerOperation(Summary = "all user projects and task")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user projects and task", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AllProjectWithTask()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _userService.AllProjectWithTask(userId);
            return Ok(response);
        }


        [HttpPost("add-user-task", Name = "add-user-task")]
        [SwaggerOperation(Summary = "add a user to a task")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user task", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Task Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AddUserToTask([FromBody] UserTaskRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _userService.AddUserToTask(userId, request);
            return Ok(response);
        }


        [HttpPost("pick-task", Name = "pick-task")]
        [SwaggerOperation(Summary = "user picks a task")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user task", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Task Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> PickTask(string taskId)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _userService.PickTask(userId, taskId);
            return Ok(response);
        }


        [HttpGet("user-notifications", Name = "user-notifications")]
        [SwaggerOperation(Summary = "All user's notifications")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "Notifications", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AllNotifications()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _noteService.GetNotifications(userId);
            return Ok(response);
        }
    }
}
