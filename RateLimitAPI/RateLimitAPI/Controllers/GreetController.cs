using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RateLimitAPI.Filters;
using RateLimitAPI.Model;

namespace RateLimitAPI.Controllers
{
    [Produces("application/json")]
    [Route("greet")]
    public class GreetController : Controller
    {
        private readonly IMemoryCache cache;
        private readonly IHttpContextAccessor httpContextAccessor; 

        public GreetController(
            IMemoryCache cache,
            IHttpContextAccessor httpContextAccessor)
        {
            this.cache = cache;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("{name}")]
        [ServiceFilter(typeof(RateLimitFilter))]
        public IActionResult Get(string name)
        {
            AddRateLimitHeaders();

            return Ok("Hi, " + name);
        }

        private void AddRateLimitHeaders()
        {
            if (cache.TryGetValue<CacheEntry>(Request.HttpContext.Connection.LocalIpAddress.Address, out var cacheEntry))
            {
                httpContextAccessor.HttpContext.Response.Headers.Add("X-RateLimit-Limit", 10.ToString());
                httpContextAccessor.HttpContext.Response.Headers.Add("X-RateLimit-Remaining", (10 - cacheEntry.NumberOfRequests).ToString());
            }
        }
    }
}