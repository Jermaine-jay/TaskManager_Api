using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Models.Dtos;
using TaskManager.Services.Interfaces;


namespace TaskManager.Test.Admin
{
    [TestClass]
    public class AdminControllerTest
    {
        private Mock<IAdminService> _mockAdminService;
        private Mock<IProjectService> _mockProjectService;
        private Fixture _fixture;
        private readonly AdminController _adminController;


        public AdminControllerTest()
        {
            _fixture = new Fixture();
            _mockAdminService = new Mock<IAdminService>();
            _mockProjectService = new Mock<IProjectService>();
            _adminController = new AdminController(_mockAdminService.Object, _mockProjectService.Object);

        }

        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_GetUsers_ShouldReturnOk()
        {
            var users = _fixture.CreateMany<ApplicationUserDto>(5).ToList();
            _mockAdminService.Setup(repo => repo.GetUsers()).ReturnsAsync(users);

            var result = await _adminController.GetAllUsers();
            var obj = result as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(users, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_GetUsers_ShouldThrowException_WhenAdminServiceThrowsException()
        {
            _mockAdminService.Setup(repo => repo.GetUsers()).ReturnsAsync(It.IsNotIn<IEnumerable<ApplicationUserDto>>());

            var result = await _adminController.GetAllUsers();
            var obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreEqual(null, obj.Value);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_GetUser_ShouldReturnOk()
        {
            var user = _fixture.Create<ApplicationUserDto>();
            _mockAdminService.Setup(repo => repo.GetUser(It.IsAny<string>())).ReturnsAsync(user);

            var result = await _adminController.GetUser(user.Id);
            var obj = result as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(user, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_GetUser_ShouldThrowException_WhenAdminServiceThrowsException()
        {
            var user = _fixture.Create<ApplicationUserDto>();
            _mockAdminService.Setup(repo => repo.GetUser(It.IsNotIn<string>())).ReturnsAsync(It.IsNotIn<ApplicationUserDto>());

            var result = await _adminController.GetUser(It.IsNotIn<string>());
            var obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreNotEqual(user, obj.Value);
            Assert.AreNotSame(user, obj.Value);
        }
    }
}
