using Microsoft.AspNetCore.Mvc;
using RateLimitAPI.Filters;

namespace RateLimitAPI.Controllers
{
    [Produces("application/json")]
    [Route("greet")]
    public class GreetController : Controller
    {
        [HttpGet]
        [Route("{name}")]
        [ServiceFilter(typeof(RateLimitFilter))]
        public IActionResult Get(string name)
        {
            return Ok("Hi, " + name);
        }
    }
}