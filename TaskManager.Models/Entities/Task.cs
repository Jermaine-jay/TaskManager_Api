using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TaskManager.Models.Enums;

namespace TaskManager.Models.Entities
{
    public class Task : BaseEntity
    {
        [Required]
        [MaxLength(100, ErrorMessage ="Title should be less than 100 words")]
        public string Title { get; set; }

        [MaxLength(600, ErrorMessage ="Not more than 500 words")]
        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public Priority Priority { get; set; } = Priority.Low;

        public Status Status { get; set; } = Status.Pending;

        [ForeignKey("Project")]
        public Guid? ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
