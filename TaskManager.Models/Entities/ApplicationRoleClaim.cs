using Microsoft.AspNetCore.Identity;

namespace TaskManager.Models.Entities
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public bool Active { get; set; } = true;
    }
}
