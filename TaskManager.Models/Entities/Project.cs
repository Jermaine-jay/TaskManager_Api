using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TaskManager.Models.Entities
{
    public class Project : BaseEntity
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
        
        [ForeignKey("ApplicationUser")]
        public Guid? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
