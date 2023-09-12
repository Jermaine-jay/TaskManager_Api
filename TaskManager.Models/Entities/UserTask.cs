using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models.Entities
{
    public class UserTask : BaseEntity
    {
        [ForeignKey("ApplicationUser")]
        public Guid? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public Guid TaskId { get; set; }
    }
}
