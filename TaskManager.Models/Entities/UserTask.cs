using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models.Entities
{
    public class UserTask : BaseEntity
    {
        [ForeignKey("ApplicationUser")]
        public Guid? UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Task")]
        public Guid? TaskId { get; set; }
        public Task Task { get; set; }
    }
}
