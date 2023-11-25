using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.Dtos.Request
{
        public class UserRegistrationRequest
        {
            [Required]
            public string? FirstName { get; set; }

            [Required]
            public string? LastName { get; set; }

            [Required, DataType(DataType.Password)]
            public string? Password { get; set; }

            [Required, DataType(DataType.Password), Compare(nameof(Password))]
            public string? ConfirmPassword { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string? Email { get; set; }

            [Phone]
            public string? PhoneNumber { get; set; }

        }
    
}
