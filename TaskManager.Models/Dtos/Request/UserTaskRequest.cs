﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Dtos.Request
{
    public class UserTaskRequest
    {
        public List<string>? UsersId { get; set; }
        public string? TaskId { get; set; }
    }
}
