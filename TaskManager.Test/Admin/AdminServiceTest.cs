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
using TaskManager.Models.Entities;
using TaskManager.Services.Implementations;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;

namespace TaskManager.Test.Admin
{
    [TestClass]
    public class AdminServiceTest
    {
        private Mock<IRepository<ApplicationUser>> _mockUserRepo;
        private Mock<IRepository<Project>> _mockProjectRepo;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IUnitOfWork> _unitOfWork;
        private Fixture _fixture;
        private readonly AdminService _adminService;
        public AdminServiceTest()
        {
            _fixture = new Fixture();       
            _mockUserRepo = new Mock<IRepository<ApplicationUser>>();
            _mockProjectRepo = new Mock<IRepository<Project>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _adminService = new AdminService(_mockUserManager.Object, _unitOfWork.Object);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task AdminService_GetUser_ShouldReturnOk()
        {
            //var user = _fixture.Create<SuccessResponse>();
            var users = _fixture.CreateMany<SuccessResponse>(5).ToList();
           // _mockProjectRepo.Setup(repo => await repo.GetAllAsync([Func<])).ReturnsAsync(users);

            var result = await _adminService.UsersProjectsWithTasks();
            var obj = result.Data as OkObjectResult;

            Assert.IsNotNull(obj.Value);
            Assert.AreEqual(users.FirstOrDefault().Data, obj.Value);
            Assert.AreEqual(200, obj.StatusCode);
        }
    }
}
