using Microsoft.AspNetCore.Mvc;

namespace RateLimitAPI.Controllers
{
    [Produces("application/json")]
    [Route("greet")]
    public class GreetController : Controller
    {
        [HttpGet]
        [Route("{name}")]
        public IActionResult Get(string name)
        {
            return Ok("Hi, " + name);
        }
    }
}