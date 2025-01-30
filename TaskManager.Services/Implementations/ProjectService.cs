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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IUnitOfWork _unitOfWork;


        public ProjectService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _projectRepo = _unitOfWork.GetRepository<Project>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }

        public async Task<CreateTaskResponse> CreateProject(string userId, CreateProjectRequest request)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            Project? project = await _projectRepo.GetSingleByAsync(p => p.Name ==  request.Name.ToLower());
            if (project != null)
                throw new InvalidOperationException("Project Name already exist");


            Project newProj = new Project
            {
                Name = request.Name.ToLower(),
                Description = request.Description,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,    
            };

            await _projectRepo.AddAsync(newProj);
            return new CreateTaskResponse
            {
                Message = "Project Created",
                Status = HttpStatusCode.Created,
                Success = true,
                Data = user,

            };
        }

        public async Task<SuccessResponse> DeleteProject(string userId, string projectId)
        {
            ApplicationUser user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User does not exist");

            Project project = await _projectRepo.GetSingleByAsync(u => u.Id.ToString() == projectId);
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            if(user.Projects.Any(x=> x.Id != project.Id))
                throw new InvalidOperationException("You are not allowed to perform this action");

            await _projectRepo.DeleteAsync(project);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<SuccessResponse> DeleteProjects(string userId)
        {
            ApplicationUser user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User does not exist");

            List<Project> itemList = user.Projects.ToList()??
                throw new InvalidOperationException("No project");

            await System.Threading.Tasks.Task.WhenAll(itemList.Select( item => _projectRepo.DeleteAsync(item)));

            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<SuccessResponse> UpdateProject(string userId, UpdateProjectRequest request)
        {
            ApplicationUser user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId, include: u => u.Include(u => u.Projects));
            if (user == null)
                throw new InvalidOperationException("User does not exist");

            Project project = await _projectRepo.GetSingleByAsync(u => u.Id.ToString() == request.ProjectId);
            if (project == null)
                throw new InvalidOperationException("Project does not exist");

            if (user.Projects.Any(x => x.Id != project.Id))
                throw new InvalidOperationException("You are not allowed to perform this action");

            Project newProj = new Project
            {
                Id = project.Id,
                Name = request.Name,
                Description = request.Description,
                UpdatedAt = DateTime.UtcNow
            };

            await _projectRepo.UpdateAsync(newProj);
            return new SuccessResponse
            {
                Success = true,
                Data = newProj
            };
        }
    }
}
