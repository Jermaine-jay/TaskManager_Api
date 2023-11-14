using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Api.Controllers;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Models.Entities;
using TaskManager.Services.Implementations;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;

namespace TaskManager.Test.Auth
{
    [TestClass]
    public class AuthControllerTest
    {
  
        private Mock<IAuthService> _mockAuthService;
        private Mock<IProjectService> _mockProjectService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Fixture _fixture;
        private readonly AuthController _authController;


        public AuthControllerTest()
        {
            _fixture = new Fixture();
            _mockAuthService = new Mock<IAuthService>();
            _mockProjectService = new Mock<IProjectService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _authController = new AuthController(_mockAuthService.Object);
           
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AuthController_CreateUser_ShouldReturnOk()
        {
            var user = _fixture.Create<UserRegistrationRequest>();
            var request = _fixture.Create<ServiceResponse<SuccessResponse>>();
            _mockAuthService.Setup(repo => repo.RegisterUser(user)).ReturnsAsync(request);

            var result = await _authController.CreateUser(user);
            var obj = result as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(request, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AuthController_CreateUsers_ShouldThrowException_WhenAuthServiceThrowsException()
        {
            var user = _fixture.Create<UserRegistrationRequest>();
            _mockAuthService.Setup(repo => repo.RegisterUser(user)).ThrowsAsync(new InvalidOperationException("Failed to create user"));

            var result = await _authController.CreateUser(It.IsAny<UserRegistrationRequest>());
            var obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreEqual(null, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task Authontroller_LoginUser_ShouldReturnOk()
        {
            var request = _fixture.Create<LoginRequest>();
            var response = _fixture.Create<AuthenticationResponse>();
            _mockAuthService.Setup(repo => repo.UserLogin(request)).ReturnsAsync(response);

            var result = await _authController.LoginUser(request);
            var obj = result as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(response, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
            Assert.AreSame(result, response);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_LoginUser_ShouldThrowException_WhenAutherviceThrowsException()
        {
            var request = _fixture.Create<LoginRequest>();
            var response = _fixture.Create<AuthenticationResponse>();
            _mockAuthService.Setup(repo => repo.UserLogin(request)).ThrowsAsync(new InvalidOperationException("Invalid username or password"));

            var result = await _authController.LoginUser(It.IsAny<LoginRequest>());
            var obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreNotEqual(response, obj.Value);
            Assert.AreNotSame(result, response);
        }
    }
}
