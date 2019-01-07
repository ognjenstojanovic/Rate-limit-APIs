using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RateLimitAPI.Util
{
    public class TooManyRequestsResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var result = new ObjectResult("Rate limit exceeded.")
            {
                StatusCode = 429
            };

            return result.ExecuteResultAsync(context);
        }
    }
}
