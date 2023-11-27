using TaskManager.Models.Entities;

namespace TaskManager.Models.Dtos.Response
{
    public class AddUserToRoleResponse
    {

        public string? Message { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
    }

    public class RoleResponse
    {
        public string? Name { get; set; }
        public IEnumerable<ApplicationRoleClaim> Claims { get; set; }
        public bool Active { get; set; }


    }
}
