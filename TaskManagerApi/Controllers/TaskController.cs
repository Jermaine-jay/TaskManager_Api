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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Authorization")]
    public class TaskController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITaskService _taskService;

        public TaskController(IHttpContextAccessor contextAccessor, ITaskService taskService)
        {
            _httpContextAccessor = contextAccessor;
            _taskService = taskService;
        }
    
        [HttpPost("create-task", Name = "create-task")]
        [SwaggerOperation(Summary = "Create new task")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "Task", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Project does not exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Task Tile already Exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
           
            SuccessResponse response = await _taskService.CreateTask(userId, request);
            return Ok(response);
        }

   
        [HttpDelete("delete-task", Name = "delete-task")]
        [SwaggerOperation(Summary = "delete a task")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "Task", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            SuccessResponse response = await _taskService.DeleteTask(taskId, userId);
            return Ok(response);
        }

      
        [HttpPut("update-task", Name = "update-task")]
        [SwaggerOperation(Summary = "update a task")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "Task", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _taskService.UpdateTask(userId, request);
            return Ok(response);
        }

    
        [HttpPut("update-priority", Name = "update-priority")]
        [SwaggerOperation(Summary = "update task priority")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "Tak", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdatePriority([FromBody] UpdatePriorityRequest request)
        {

            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _taskService.UpdatePriority(userId,request);
            return Ok(response);
        }


        [HttpPut("update-status", Name = "update-status")]
        [SwaggerOperation(Summary = "update task status")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "update existing task", Type = typeof(UpdateTaskResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Project name already exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {

            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            UpdateTaskResponse response = await _taskService.UpdateStatus(userId,request);
            return Ok(response);
        }
    }
}
