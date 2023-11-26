using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Dtos.Request
{
    public class LockUserRequest
    {
        public string? UserId { get; set; }
        public int Duration { get; set; }
    }
}
