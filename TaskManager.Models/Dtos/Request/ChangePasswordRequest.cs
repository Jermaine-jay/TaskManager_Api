using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.Dtos.Request
{
    public class ChangePasswordRequest
    {

        [Required, DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Required, DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
