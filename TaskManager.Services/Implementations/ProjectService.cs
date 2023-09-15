using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using Task = TaskManager.Models.Entities.Task;


namespace TaskManager.Services.Implementations
{
    public class ProjectService : IProjectService
    {

        private readonly IServiceFactory _serviceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IUnitOfWork _unitOfWork;


        public ProjectService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
            _userManager = userManager;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }


        public async Task<CreateTaskResponse> CreateProject(string userId, CreateProjectRequest request)
        {
            var user = await _userRepo.GetSingleByAsync(u => u.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            var project = await _projectRepo.GetSingleByAsync(p => p.Equals(request.Name.ToLower()));
            if (project == null)
                throw new InvalidOperationException("Project Name already exist");


            var newProj = new Project
            {
                Name = request.Name.ToLower(),
                Description = request.Description,
                UserId = user.Id,
            };

            await _projectRepo.AddAsync(newProj);
            //user.Projects.Add(newProj);
            return new CreateTaskResponse
            {
                Message = "Project Created",
                Status = HttpStatusCode.Created,
                Success = true,
                Data = newProj,

            };
        }

        public async Task<SuccessResponse> DeleteProject(string userId, string projectId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User does not exist");

            var project = user.Projects.Where(u => u.Id.ToString() == projectId).FirstOrDefault();
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            await _projectRepo.DeleteAsync(project);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<SuccessResponse> UpdateProject(string userId, UpdateProjectRequest request)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User does not exist");

            var project = user.Projects.Where(u => u.Id.ToString() == request.ProjectId).FirstOrDefault();
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            var newProj = new Project
            {
                Name = request.Name,
                Description = request.Description
            };

            await _projectRepo.UpdateAsync(newProj);
            return new SuccessResponse
            {
                Success = true
            };
        }
    }
}
