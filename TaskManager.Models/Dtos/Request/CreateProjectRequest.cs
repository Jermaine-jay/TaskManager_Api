using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Dtos.Request
{
    public class CreateProjectRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
