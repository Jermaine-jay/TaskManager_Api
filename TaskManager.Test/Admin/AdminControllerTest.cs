using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos;
using TaskManager.Models.Entities;
using TaskManager.Services.Implementations;
using TaskManager.Services.Interfaces;


namespace TaskManager.Test.Admin
{
    [TestClass]
    public class AdminControllerTest
    {
        private Mock<IAdminService> _mockAdminService;
        private Mock<IRepository<ApplicationUser>> _mockUserRepo;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IUnitOfWork> _unitOfWork;
        private Fixture _fixture;
        private readonly AdminController _adminController;
        private readonly AdminService _adminService;
        public AdminControllerTest()
        {
            _fixture = new Fixture();
            _mockAdminService = new Mock<IAdminService>();
            _mockUserRepo = new Mock<IRepository<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _adminController = new AdminController(_mockAdminService.Object);
            _adminService = new AdminService(_mockUserManager.Object, _unitOfWork.Object);

        }

        [TestMethod]
        public async System.Threading.Tasks.Task AdminController_GetUsers_ShouldReturnOk()
        {
            //var user = _fixture.Create<string>();
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
            _mockAdminService.Setup(repo => repo.GetUser(It.IsAny<string>())).ReturnsAsync(It.IsNotIn<ApplicationUserDto>());

            var result = await _adminController.GetUser(null);
            var obj = result as OkObjectResult;

            Assert.IsNull(obj.Value);
            Assert.AreEqual(null, obj.Value);
            Assert.AreEqual(400, obj.StatusCode);
        }
    }
}
