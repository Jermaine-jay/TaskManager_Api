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
            IList<ApplicationUserDto> users = _fixture.CreateMany<ApplicationUserDto>(5).ToList();
            _mockAdminService.Setup(repo => repo.GetUsers()).ReturnsAsync(users);

            IActionResult result = await _adminController.GetAllUsers();
            OkObjectResult? obj = result as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(users, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_GetUsers_ShouldThrowException_WhenAdminServiceThrowsException()
        {
            _mockAdminService.Setup(repo => repo.GetUsers()).ReturnsAsync(It.IsNotIn<IEnumerable<ApplicationUserDto>>());

            IActionResult result = await _adminController.GetAllUsers();
            OkObjectResult? obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreEqual(null, obj.Value);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_GetUser_ShouldReturnOk()
        {
            ApplicationUserDto user = _fixture.Create<ApplicationUserDto>();
            _mockAdminService.Setup(repo => repo.GetUser(It.IsAny<string>())).ReturnsAsync(user);

            IActionResult result = await _adminController.GetUser(user.Id);
            OkObjectResult? obj = result as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(user, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_GetUser_ShouldThrowException_WhenAdminServiceThrowsException()
        {
            ApplicationUserDto user = _fixture.Create<ApplicationUserDto>();
            _mockAdminService.Setup(repo => repo.GetUser(It.IsNotIn<string>())).ReturnsAsync(It.IsNotIn<ApplicationUserDto>());

            IActionResult result = await _adminController.GetUser(It.IsNotIn<string>());
            OkObjectResult? obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreNotEqual(user, obj.Value);
            Assert.AreNotSame(user, obj.Value);
        }
    }
}
