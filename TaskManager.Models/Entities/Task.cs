using System.ComponentModel.DataAnnotations;
using TaskManager.Models.Enums;

namespace TaskManager.Models.Entities
{
    public class Task : BaseEntity
    {
        [Required]
        [MaxLength(150, ErrorMessage = "Title should be less than 100 words")]
        public string Title { get; set; }

        public string Description { get; set; }


        public DateTime DueDate { get; set; }

        public Priority Priority { get; set; } = Priority.Low;

        public Status Status { get; set; } = Status.Pending;

        public Guid? ProjectId { get; set; }

        public virtual Project? Project { get; set; }

        public virtual ICollection<UserTask>? UserTasks { get; set; }
    }
}
