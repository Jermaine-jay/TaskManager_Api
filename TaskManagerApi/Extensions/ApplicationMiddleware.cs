using TaskManager.Services.Interfaces;

namespace TaskManager.Api.Extensions
{
    public class ApplicationMiddleware : IMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ITaskService _service;

        public ApplicationMiddleware(RequestDelegate next, ITaskService service)
        {
            _next = next;
            _service = service;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
     
            await _service.AllTask();
            await _next(context);
        }
    }
}
