using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimitAPI.Model
{
    public class CacheEntry
    {
        public DateTime InsertTime { get; set; }

        public int NumberOfRequests { get; set; }

        public long UserIp { get; set; }
    }
}