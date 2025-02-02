using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;


namespace TaskManager.Test.Auth
{
    [TestClass]
    public class AuthControllerTest
    {

        private Mock<IAuthService> _mockAuthService;
        private Fixture _fixture;
        private readonly AuthController _authController;

        public AuthControllerTest()
        {
            _fixture = new Fixture();
            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockAuthService.Object);

        }

        [TestMethod]
        public async System.Threading.Tasks.Task AuthController_CreateUser_ShouldReturnOk()
        {
            UserRegistrationRequest user = _fixture.Create<UserRegistrationRequest>();
            SuccessResponse request = _fixture.Create<SuccessResponse>();
            _mockAuthService.Setup(repo => repo.RegisterUser(user)).ReturnsAsync(request);

            IActionResult result = await _authController.CreateUser(user);
            OkObjectResult? obj = result as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(request, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AuthController_CreateUsers_ShouldThrowException_WhenAuthServiceThrowsException()
        {
            UserRegistrationRequest user = _fixture.Create<UserRegistrationRequest>();
            _mockAuthService.Setup(repo => repo.RegisterUser(user)).ThrowsAsync(new InvalidOperationException("Failed to create user"));

            IActionResult result = await _authController.CreateUser(It.IsAny<UserRegistrationRequest>());
            OkObjectResult? obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreEqual(null, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task Authontroller_LoginUser_ShouldReturnOk()
        {
            LoginRequest request = _fixture.Create<LoginRequest>();
            AuthenticationResponse response = _fixture.Create<AuthenticationResponse>();
            _mockAuthService.Setup(repo => repo.UserLogin(request)).ReturnsAsync(response);

            IActionResult result = await _authController.LoginUser(request);
            OkObjectResult? obj = result as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(response, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_LoginUser_ShouldThrowException_WhenAutherviceThrowsException()
        {
            LoginRequest request = _fixture.Create<LoginRequest>();
            AuthenticationResponse response = _fixture.Create<AuthenticationResponse>();
            _mockAuthService.Setup(repo => repo.UserLogin(request)).ThrowsAsync(new InvalidOperationException("Invalid username or password"));

            IActionResult result = await _authController.LoginUser(It.IsAny<LoginRequest>());
            OkObjectResult obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreNotEqual(response, obj.Value);
            Assert.AreNotSame(result, response);
        }
    }
}
