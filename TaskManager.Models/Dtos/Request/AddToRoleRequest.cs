using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.Dtos.Request
{

    public class AddUserToRoleRequest
    {
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class RoleDto
    {
        [Required(ErrorMessage = "Role Name cannot be empty"), MinLength(2), MaxLength(30)]
        public string Name { get; set; } = null!;
    }

}
