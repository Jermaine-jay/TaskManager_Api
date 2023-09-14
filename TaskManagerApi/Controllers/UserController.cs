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
    public class UserController : ControllerBase
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public UserController(IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _httpContextAccessor = contextAccessor;
            _userService = userService;
        }


        [AllowAnonymous]
        [HttpPut("change-password", Name = "Change-password")]
        [SwaggerOperation(Summary = "Change user password")]
        [SwaggerResponse(StatusCodes.Status202Accepted, Description = "User", Type = typeof(ChangePasswordResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.ChangePassword(userId, request);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpDelete("delete-user", Name = "delete-user")]
        [SwaggerOperation(Summary = "delete a user")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteUser()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.DeleteUser(userId);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPut("update-user", Name = "update-user")]
        [SwaggerOperation(Summary = "update a user")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.UpdateUser(userId, request);
            return Ok(response);
        }      
    }
}
