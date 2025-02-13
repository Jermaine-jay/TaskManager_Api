﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Models.Entities;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceFactory _serviceFactory;
        private readonly IRepository<ApplicationRole> _roleRepo;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationRoleClaim> _roleClaimRepo;

        public RoleService(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            _unitOfWork = _serviceFactory.GetService<IUnitOfWork>();
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
            _roleClaimRepo = _unitOfWork.GetRepository<ApplicationRoleClaim>();
            _userManager = _serviceFactory.GetService<UserManager<ApplicationUser>>();
            _roleManager = _serviceFactory.GetService<RoleManager<ApplicationRole>>();
        }

        public async Task<AddUserToRoleResponse> AddUserToRole(AddUserToRoleRequest request)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(request.Email.Trim().ToLower());
            if (user == null)
                throw new InvalidOperationException("Project does not exist");

            ApplicationRole? role = await _roleManager.FindByNameAsync(request.Role.ToLower().Trim());
            if (role == null)
                throw new InvalidOperationException("Project does not exist");

            await _userManager.AddToRoleAsync(user, role.Name);
            return new AddUserToRoleResponse
            {
                Message = $"{user} added to {role.Name}",
                UserName = user.FirstName,
            };
        }

        public async Task<SuccessResponse> CreateRoleAync(RoleDto request)
        {
            ApplicationRole? role = await _roleManager.FindByNameAsync(request.Name.Trim().ToLower());
            if (role != null)
                throw new InvalidOperationException("Project does not exist");

            ApplicationRole applicationRole = new()
            {
                Name = request.Name,
            };

            await _roleManager.CreateAsync(applicationRole);
            return new SuccessResponse
            {
                Success = true,
                Data = applicationRole
            };
        }

        public async Task<SuccessResponse> DeleteRole(string name)
        {
            ApplicationRole? role = await _roleManager.FindByNameAsync(name.Trim().ToLower());
            if (role == null)
                throw new InvalidOperationException("Project does not exist");

            await _roleManager.DeleteAsync(role);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<SuccessResponse> EditRole(string id, string Name)
        {
            ApplicationRole? role = await _roleManager.FindByNameAsync(id.Trim().ToLower());
            if (role == null)
                throw new InvalidOperationException("Project does not exist");

            role.Name = Name;
            await _roleManager.UpdateAsync(role);

            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<SuccessResponse> RemoveUserFromRole(AddUserToRoleRequest request)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(request.Email.Trim().ToLower());
            if (user == null)
                throw new InvalidOperationException("Project does not exist");

            IQueryable<string> myRoles = _roleManager.Roles.Select(x => x.Name);
            if (!myRoles.Contains(request.Role))
            {
                return new SuccessResponse
                {
                    Success = false
                };
            }

            IdentityResult userIsInRole = await _userManager.RemoveFromRoleAsync(user, request.Role);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<IEnumerable<string>> GetUserRoles(string userName)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new InvalidOperationException("Project does not exist");

            List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);
            if (!userRoles.Any())
            {
                return userRoles;
            }

            return userRoles;
        }

        public async Task<IEnumerable<RoleResponse>> GetAllRoles()
        {
            IEnumerable<ApplicationRole> roleQueryable = await _roleRepo.GetAllAsync(include: u => u.Include(x => x.RoleClaims));
            roleQueryable = roleQueryable.Where(r => r.Active);

            IEnumerable<RoleResponse> roleResponseQueryable = roleQueryable.Select(s => new RoleResponse
            {
                Name = s.Name,
                Claims = s.RoleClaims.Where(r => r.ClaimValue.ToLower() is not null && r.Active),
                Active = s.Active
            });

            return roleResponseQueryable;
        }
    } 
}
