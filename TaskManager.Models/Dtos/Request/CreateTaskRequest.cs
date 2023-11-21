using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.Dtos.Request
{
    public class CreateTaskRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string? DueDate { get; set; }

        public int? Priority { get; set; }

        public int? Status { get; set; }

        public string? ProjectId { get; set; }
    }
}
