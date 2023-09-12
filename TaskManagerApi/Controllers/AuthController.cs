using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.InteropServices;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using static TaskManager.Services.Implementations.AuthService;


namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAuthService _authService;

        public AuthController(IHttpContextAccessor contextAccessor, IAuthService authService)
        {
            _contextAccessor = contextAccessor;
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpPost("create-new-user", Name = "Create-New-User")]
        [SwaggerOperation(Summary = "Creates user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "UserId of created user")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User with provided email already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateUser([FromBody] UserRegistrationRequest request)
        {
            var response = await _authService.RegisterUser(request);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("confirm-email", Name = "Confirm-email")]
        [SwaggerOperation(Summary = "Confirms a user's email")]
        [SwaggerResponse(StatusCodes.Status202Accepted, Description = "User", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid Operation", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid Token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ConfirmEmail(string validToken)
        {
            var response = await _authService.ConfirmEmail(validToken);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("login", Name = "login")]
        [SwaggerOperation(Summary = "Authenticates user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns user Id", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid username or password", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest loginRequest)
        {
            var response = await _authService.UserLogin(loginRequest);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("signin-google", Name = "signin-google")]
        [SwaggerOperation(Summary = "Authenticates user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns user Id", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid username or password", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GoogleLogin([FromBody] ExternalAuthRequest loginRequest)
        {
            var response = await _authService.GoogleAuth(loginRequest);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPost("forgot-password", Name = "forgot-password")]
        [SwaggerOperation(Summary = "forgot-password")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns a token", Type = typeof(ChangePasswordResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid email", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User does not exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var response = await _authService.ForgotPassword(request);

            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("reset-password", Name = "reset-password")]
        [SwaggerOperation(Summary = "reset-password")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns a token", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User does not exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid Token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var response = await _authService.ResetPassword(request);
            if (response.Success)
                return RedirectToAction("LoginUser");

            return BadRequest(response);
        }
    }
}
