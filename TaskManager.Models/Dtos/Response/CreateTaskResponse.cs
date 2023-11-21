using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Dtos.Response
{
    public class CreateTaskResponse
    {
        public string? Message { get; set; }
        public bool? Success { get; set; }
        public HttpStatusCode? Status { get; set; }
        public object? Data { get; set; }
    }
}
