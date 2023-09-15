using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models.Entities
{
    public class UserTask
    {
        [ForeignKey("ApplicationUser")]
        public Guid? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("Task")]
        public Guid? TaskId { get; set; }
        public virtual Task? Task { get; set; }
    }
}
