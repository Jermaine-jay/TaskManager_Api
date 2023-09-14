using System.Net;

namespace TaskManager.Models.Dtos.Response
{
    public class UpdateTaskResponse
    {
        public string? Message { get; set; }
        public bool? Success { get; set; }
        public HttpStatusCode? Status { get; set; }
        public object? Data { get; set; }
    }
}
