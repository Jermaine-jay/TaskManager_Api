using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TaskManager.Models.Entities
{
    public class Project : BaseEntity
    {

        [Required]
        [MaxLength(100, ErrorMessage = "Name must be less than 100 letters")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "Text must be less than 500 words")]
        public string? Description { get; set; }


        [ForeignKey("ApplicationUser")]
        public Guid? ApplicationUserId { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }

        public virtual ICollection<Task>? Tasks { get; set; }
    }
}
