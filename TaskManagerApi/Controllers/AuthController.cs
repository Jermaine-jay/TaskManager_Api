using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Models.Dtos;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;


namespace TaskManager.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("create-user", Name = "create-User")]
        [SwaggerOperation(Summary = "Creates user")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "UserId of created user", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User with provided email already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateUser([FromBody] UserRegistrationRequest request)
        {
            var response = await _authService.RegisterUser(request);
            return Ok(response);
        }



        [HttpGet("confirm-email", Name = "confirm-email")]
        [SwaggerOperation(Summary = "Confirms a user's email")]
        [SwaggerResponse(StatusCodes.Status202Accepted, Description = "User", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid Operation", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid Token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var response = await _authService.ConfirmEmail(token);
            return Ok(response);
        }


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


        [HttpPost("signin-google", Name = "signin-google")]
        [SwaggerOperation(Summary = "Authenticates user with Google")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns user Id", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid username or password", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GoogleLogin([FromBody] ExternalAuthRequest loginRequest)
        {
            var response = await _authService.GoogleAuth(loginRequest);
            return Ok(response);
        }


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


        [HttpPut("reset-password", Name = "reset-password")]
        [SwaggerOperation(Summary = "reset-password")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns a token", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User does not exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid Token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ResetPassword([FromQuery] ResetPasswordRequest request)
        {
            var response = await _authService.ResetPassword(request);
            if (response.Success)
                return RedirectToAction("LoginUser");

            return BadRequest(response);
        }
    }
}
