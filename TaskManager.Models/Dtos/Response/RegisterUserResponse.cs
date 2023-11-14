using TaskManager.Models.Enums;

namespace TaskManager.Models.Dtos.Response
{
    public class RegisterUserResponse
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public UserType UserType { get; set; }
    }
}
