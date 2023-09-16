using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TaskManager.Models.Entities
{
    public class Project : BaseEntity
    {

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(800)]
        public string Description { get; set; }

        public Guid? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Task>? Tasks { get; set; }
    }
}
