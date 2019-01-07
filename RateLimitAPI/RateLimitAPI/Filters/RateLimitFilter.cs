using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using RateLimitAPI.Model;
using RateLimitAPI.Util;
using System;

namespace RateLimitAPI.Filters
{
    public class RateLimitFilter : IActionFilter
    {
        private readonly IMemoryCache cache;

        public RateLimitFilter(IMemoryCache cache)
        {
            this.cache = cache;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            var userIp = context.HttpContext.Connection.LocalIpAddress.Address;

            if (cache.TryGetValue(userIp, out var cacheEntry))
            {
                var ce = (CacheEntry)cacheEntry;

                if (DateTime.UtcNow - ce.InsertTime > TimeSpan.FromMinutes(1))
                {
                    cache.Remove(userIp);
                    InsertIntoCache(userIp);
                }
                else
                {
                    if (ce.NumberOfRequests >= 10)
                    {
                        context.Result = new TooManyRequestsResult();
                    }
                    else
                    {
                        UpdateCache(ce);
                    }
                }
            }
            else
            {
                InsertIntoCache(userIp);
            }            
        }

        private void InsertIntoCache(long userIp)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(1));
            
            var ce = new CacheEntry { NumberOfRequests = 1, InsertTime = DateTime.UtcNow, UserIp = userIp };
            cache.Set(userIp, ce, cacheEntryOptions);
        }

        private void UpdateCache(CacheEntry ce)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(DateTime.UtcNow - ce.InsertTime);

            ce.NumberOfRequests++;
            cache.Set(ce.UserIp, ce, cacheEntryOptions);
        }
    }
}
