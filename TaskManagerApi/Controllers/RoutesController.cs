using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;
using TaskManager.Services.Infrastructure;


namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IEnumerable<EndpointDataSource> _endpointSources;

        public RoutesController(IEnumerable<EndpointDataSource> endpointSources)
        {
            _endpointSources = endpointSources;
        }



        [HttpGet("get-all-routes", Name = "get-all-routes")]
        [SwaggerOperation(Summary = "Gets all routes ")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Routes Retrieved")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, Description = "Unauthorized User", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetRoutes()
        {
            var endpoints = _endpointSources.SelectMany(es => es.Endpoints)
            .OfType<RouteEndpoint>();
            IEnumerable<string?> output = endpoints.Select(e =>
            {
                var controller = e.Metadata
                    .OfType<ControllerActionDescriptor>()
                    .FirstOrDefault();
                var httpMethod = controller?.MethodInfo
                    .GetCustomAttributes<HttpMethodAttribute>()
                    .FirstOrDefault()
                    ?.Name;

                return httpMethod;
            });
            return Ok(output);
        }
    }
}
