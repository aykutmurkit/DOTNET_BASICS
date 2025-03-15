namespace test.Configuration
{
    /// <summary>
    /// Configuration for API rate limiting
    /// </summary>
    public class RateLimitingConfiguration
    {
        /// <summary>
        /// Maximum number of requests allowed per time window
        /// </summary>
        public int RequestLimit { get; set; } = 100;

        /// <summary>
        /// Time window in minutes
        /// </summary>
        public int TimeWindowMinutes { get; set; } = 1;
    }
} 