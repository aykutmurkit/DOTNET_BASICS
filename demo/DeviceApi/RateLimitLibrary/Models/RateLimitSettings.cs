using System.Collections.Generic;

namespace RateLimitLibrary.Models
{
    public class RateLimitSettings
    {
        public bool EnableGlobalRateLimit { get; set; }
        public string GlobalRateLimitPeriod { get; set; }
        public int GlobalRateLimitRequests { get; set; }
        public IpRateLimitingSettings IpRateLimiting { get; set; } = new();
        public List<EndpointRateLimit> EndpointLimits { get; set; } = new();
    }

    public class IpRateLimitingSettings
    {
        public bool EnableIpRateLimiting { get; set; }
        public string IpRateLimitPeriod { get; set; }
        public int IpRateLimitRequests { get; set; }
    }

    public class EndpointRateLimit
    {
        public string Endpoint { get; set; }
        public string Period { get; set; }
        public int Limit { get; set; }
        public bool EnableConcurrencyLimit { get; set; }
        public int ConcurrencyLimit { get; set; }
    }
} 