using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Api.Extensions;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Implementations;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjectService _projectService;

        public ProjectController(IHttpContextAccessor contextAccessor, IProjectService projectService)
        {
            _httpContextAccessor = contextAccessor;
            _projectService = projectService;
        }

        

        [HttpPost("create-project", Name = "create-project")]
        [SwaggerOperation(Summary = "Create new project")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "project", Type = typeof(CreateTaskResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Project name already Exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _projectService.CreateProject(userId, request);
            return Ok(response);
        }



        [HttpDelete("delete-project", Name = "delete-project")]
        [SwaggerOperation(Summary = "Delete a project")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "Tak", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Project Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteProject(string projectId)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _projectService.DeleteProject(userId, projectId);
            return Ok(response);
        }



        [HttpPut("update-project", Name = "update-project")]
        [SwaggerOperation(Summary = "update a project")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "project", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Project Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _projectService.UpdateProject(userId, request);
            return Ok(response);
        }
    }
}
