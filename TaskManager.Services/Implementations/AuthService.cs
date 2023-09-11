using MailKit;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Services.Configurations.Cache.Otp;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using TaskManager.Services.Utilities;

namespace TaskManager.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtAuthenticator _jwtAuthenticator;

        public AuthService(IJwtAuthenticator jwtAuthenticator, IServiceFactory serviceFactory, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
            _serviceFactory = serviceFactory;
            _userManager = userManager;
            _jwtAuthenticator = jwtAuthenticator;
        }


        public async Task<ServiceResponse<SuccessResponse>> RegisterUser(UserRegistrationRequest request)
        {
            ApplicationUser? existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"User already exists with Email {request.Email}");


            var emailExist = await _userManager.FindByNameAsync(request.Email);
            if (emailExist != null)
                throw new InvalidOperationException($"User already exists");


            var verifyEmail = await _serviceFactory.GetService<IEmailService>().VerifyEmailAddress(request.Email);
            if (!verifyEmail)
                throw new InvalidOperationException($"Email {request.Email} is invalid");


            ApplicationUser user = new()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.Firstname,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Active = true,
                UserType = UserType.User,
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var message = $"Failed to create user: {(result.Errors.FirstOrDefault())?.Description}";
                throw new InvalidOperationException(message);
            }


            var registerMail = await _serviceFactory.GetService<IEmailService>().RegistrationMail(user);
            if (!registerMail)
                throw new InvalidOperationException($"Could not send verification mail");


            var role = UserType.User.GetStringValue();
            bool roleExists = await _roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                ApplicationRole newRole = new ApplicationRole { Name = role };
                await _roleManager.CreateAsync(newRole);
            }

            await _userManager.AddToRoleAsync(user, role);
            var response = new ServiceResponse<SuccessResponse>
            {
                Message = "User created Sucessfully",
                StatusCode = HttpStatusCode.Created,
                Data = new SuccessResponse
                {
                    Success = true,
                    Data = user
                }

            };
            return response;
        }


        public async Task<SuccessResponse> ConfirmEmail(string validToken)
        {
            var (existingUser, operation) = await DecodeToken.DecodeVerificationToken(validToken);

            var user = await _userManager.FindByIdAsync(existingUser);
            if (user == null)
                throw new InvalidOperationException($"User Not Found");


            if (operation != OtpOperation.EmailConfirmation.ToString())
                throw new InvalidOperationException($"Invalid Operation");


            var verifyToken = await _serviceFactory.GetService<IOtpService>().VerifyUniqueOtpAsync(user.Id.ToString(), validToken, OtpOperation.EmailConfirmation);
            if (!verifyToken)
            {
                throw new InvalidOperationException($"Invalid Token");
            }

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return new SuccessResponse
            {
                Success = true,
                Data = user
            };
        }


        public async Task<AuthenticationResponse> UserLogin(LoginRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email.ToLower().Trim());

            if (user == null)
                throw new InvalidOperationException("Invalid username or password");

            if (!user.Active)
                throw new InvalidOperationException("Account is not active");

            bool result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
                throw new InvalidOperationException("Invalid username or password");

            JwtToken userToken = await _jwtAuthenticator.GenerateJwtToken(user);
            string? userType = user.UserType.GetStringValue();


            string fullName = $"{user.LastName} {user.FirstName}";
            return new AuthenticationResponse
            {
                JwtToken = userToken,
                UserType = userType.Normalize(),
                FullName = fullName,
                TwoFactor = false,
                UserId = user.Id.ToString()
            };

        }


        public async Task<ChangePasswordResponse> ForgotPassword(ForgotPasswordRequest request)
        {

            var verify = await _serviceFactory.GetService<IEmailService>().VerifyEmailAddress(request.Email);
            if (verify == false)
                throw new InvalidOperationException($"Invalid Email Address");


            var user = await _userManager.FindByEmailAsync(request.Email);
            var isConfrimed = await _userManager.IsEmailConfirmedAsync(user);
            if (user == null || !isConfrimed)
                throw new InvalidOperationException($"User does not exist");


            var result = await _serviceFactory.GetService<IEmailService>().ResetPasswordMail(user);
            return new ChangePasswordResponse
            {
                Message = "Token sent",
                Code = result,
                Success = true
            };
            
        }


        public async Task<SuccessResponse> ResetPassword(ResetPasswordRequest request)
        {
            var (existingUser, operation) = await DecodeToken.DecodeVerificationToken(request.Token);

            ApplicationUser user = await _userManager.FindByIdAsync(existingUser);
            if (user == null || !user.EmailConfirmed)
                throw new InvalidOperationException($"User does not exist");


            if (operation != OtpOperation.PasswordReset.ToString())
                throw new InvalidOperationException($"Invalid Operation");


            bool isOtpValid = await _serviceFactory.GetService<IOtpService>().VerifyUniqueOtpAsync(user.Id.ToString(), request.Token, OtpOperation.PasswordReset);
            if (!isOtpValid)
                throw new InvalidOperationException($"Invalid Token");


            IdentityResult result = await _userManager.ChangePasswordAsync(user, request.NewPassword, request.ConfirmPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Could not complete operation");


            return new SuccessResponse
            {
                Success = true,
                Data = result,
            };
        }


        public class ForgotPasswordRequest
        {
            [Required, DataType(DataType.EmailAddress)]
            public string Email { get; set; }
        }


        public class ChangePasswordResponse
        {
            public string? Message { get; set; }
            public string? Code { get; set; }
            public string? Token { get; set; }
            public bool Success { get; set; }
        }

        public class ResetPasswordRequest
        {
            [Required]
            public string Token { get; set; }

            [Required, DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
            public string ConfirmPassword { get; set; }
        }
    }
}
