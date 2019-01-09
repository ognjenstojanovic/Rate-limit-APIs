using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace RateLimitAPI.TestSetup
{
    public class FakeIpAddressMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IPAddress fakeIpAddress = IPAddress.Parse("127.168.1.32");

        public FakeIpAddressMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Connection.LocalIpAddress = fakeIpAddress;

            await this.next(httpContext);
        }
    }
}
