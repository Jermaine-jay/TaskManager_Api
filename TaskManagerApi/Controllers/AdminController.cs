using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Models.Dtos;
using TaskManager.Models.Dtos.Request;
using TaskManager.Services.Implementations;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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



        [HttpGet("get-User", Name = "get-User")]
        [SwaggerOperation(Summary = "Get A Registered User")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "successful", Type = typeof(ApplicationUserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUser(string userId)
        {
            var response = await _adminService.GetUser(userId);
            return Ok(response);
        }



        [HttpPost("lock-User", Name = "lock-User")]
        [SwaggerOperation(Summary = "Block A Registered User")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> LockUser([FromBody] LockUserRequest request)
        {
            var response = await _adminService.LockUser(request);
            return Ok(response);
        }

    }
}
