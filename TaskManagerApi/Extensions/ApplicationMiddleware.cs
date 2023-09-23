using TaskManager.Services.Interfaces;

namespace TaskManager.Api.Extensions
{
    public class ApplicationMiddleware
    {

        private readonly RequestDelegate _next;

        public ApplicationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
     
            await _next(context);
        }
    }
}
