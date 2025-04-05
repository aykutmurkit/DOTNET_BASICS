using Microsoft.Extensions.Configuration;
using RateLimitLibrary.Extensions;
using System.Collections.Generic;

namespace RateLimitLibrary
{
    /// <summary>
    /// Main entry point for RateLimitLibrary functionality
    /// </summary>
    public class RateLimitHelper
    {
        /// <summary>
        /// Get a list of configured rate limit policies
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        /// <returns>List of rate limit policy descriptions</returns>
        public static List<string> GetConfiguredPolicies(IConfiguration configuration)
        {
            return RateLimitExtensions.GetRateLimitPolicyMappings(configuration);
        }
    }
}
