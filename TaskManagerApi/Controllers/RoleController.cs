using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;
using TaskManager.Services.Implementations;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleservice;


        public RoleController(IRoleService roleservice)
        {
            _roleservice = roleservice;
        }



 
        [HttpPost("Create-role", Name = "Create-role")]
        [SwaggerOperation(Summary = "Creates role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateRoleAync([FromBody] RoleDto request)
        {
            var response = await _roleservice.CreateRoleAync(request);
            return Ok(response);
        }



 
        [HttpPost("add-user-role", Name = "add-user-role")]
        [SwaggerOperation(Summary = "add user to role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateUserRoleAync([FromBody] AddUserToRoleRequest request)
        {
            var response = await _roleservice.AddUserToRole(request);
            return Ok(response);
        }



   
        [HttpPut("remove-user-role", Name = "remove-user-role")]
        [SwaggerOperation(Summary = "remove user from role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> RemoveUserRoleAync([FromBody] AddUserToRoleRequest request)
        {
            var response = await _roleservice.RemoveUserFromRole(request);
            return Ok(response);
        }



     
        [HttpPut("editRole", Name = "editRole")]
        [SwaggerOperation(Summary = "edit role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role does not exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to edit role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> EditRoleAsync(string id, string Name)
        {
            var response = await _roleservice.EditRole(id, Name);
            return Ok(response);
        }



     
        [HttpDelete("deleteRole", Name = "deleteRole")]
        [SwaggerOperation(Summary = "Delete role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role does not exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to delete role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteRoleAync(string Name)
        {
            var response = await _roleservice.DeleteRole(Name);
            return Ok(response);
        }



  
        [HttpGet("GetRoles", Name = "GetRoles")]
        [SwaggerOperation(Summary = "All roles")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = " ", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Empty", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AllRolesAync()
        {
            var response = await _roleservice.GetAllRoles();
            return Ok(response);
        }



        [HttpGet("GetUserRoles", Name = "GetUserRoles")]
        [SwaggerOperation(Summary = "Get user Rles")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Roles")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = " ", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Empty", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUserRolesAync(string Username)
        {
            var response = await _roleservice.GetUserRoles(Username);        
                return Ok(response);
        }

    }
}
