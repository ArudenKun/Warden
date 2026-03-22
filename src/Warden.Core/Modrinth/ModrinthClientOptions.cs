namespace Warden.Core.Modrinth;

public class ModrinthClientOptions
{
    /// <summary>
    ///     API Url of the production server
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public const string ProductionUrl = "https://api.modrinth.com/v2";

    /// <summary>
    ///     API Url of the staging server
    /// </summary>
    public const string StagingUrl = "https://staging-api.modrinth.com/v2";

    /// <summary>
    ///     The token to use for requests that require authentication
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    ///     The base URL to use for requests, you don't need to change this unless you want to use a different instance of
    ///     Modrinth (e.g., a staging server)
    ///     Default is <see cref="ProductionUrl" />
    /// </summary>
    public string BaseUrl { get; set; } = ProductionUrl;

    /// <summary>
    ///     User-Agent you want to use while communicating with Modrinth API, it's recommended to
    ///     set a uniquely identifying one (<a href="https://docs.modrinth.com/api-spec/#section/User-Agents">see the docs</a>)
    /// </summary>
    public string UserAgent { get; set; } = "Prism";

    /// <summary>
    ///     The number of times to retry a request if the rate limit is hit
    ///     Default is 5.
    /// </summary>
    public int RateLimitRetryCount { get; set; } = 5;

    /// <summary>
    ///     The maximum number of concurrent requests to send
    ///     Default is 10.
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 10;
}
