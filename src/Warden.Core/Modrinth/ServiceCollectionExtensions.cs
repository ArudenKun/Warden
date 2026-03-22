using System.Net;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Refit;

namespace Warden.Core.Modrinth;

/// <summary>
/// Extension methods for configuring Refit clients in the service collection.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the Refit clients for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddRefitClients(this IServiceCollection services)
    {
        var refitSettings = new RefitSettings();

        services
            .AddKeyedRefitClient<IProjects>(typeof(IProjects).FullName, refitSettings)
            .ConfigureModrinthHttpClient()
            .AddModrinthStandardResilienceHandler();

        services
            .AddKeyedRefitClient<IVersions>(typeof(IVersions).FullName, refitSettings)
            .ConfigureModrinthHttpClient()
            .AddModrinthStandardResilienceHandler();

        services
            .AddKeyedRefitClient<IVersionFiles>(typeof(IVersionFiles).FullName, refitSettings)
            .ConfigureModrinthHttpClient()
            .AddModrinthStandardResilienceHandler();

        services
            .AddKeyedRefitClient<IUsers>(typeof(IUsers).FullName, refitSettings)
            .ConfigureModrinthHttpClient()
            .AddModrinthStandardResilienceHandler();

        services
            .AddKeyedRefitClient<INotifications>(typeof(INotifications).FullName, refitSettings)
            .ConfigureModrinthHttpClient()
            .AddModrinthStandardResilienceHandler();

        services
            .AddKeyedRefitClient<IThreads>(typeof(IThreads).FullName, refitSettings)
            .ConfigureModrinthHttpClient()
            .AddModrinthStandardResilienceHandler();

        services
            .AddKeyedRefitClient<ITeams>(typeof(ITeams).FullName, refitSettings)
            .ConfigureModrinthHttpClient()
            .AddModrinthStandardResilienceHandler();

        services
            .AddKeyedRefitClient<ITags>(typeof(ITags).FullName, refitSettings)
            .ConfigureModrinthHttpClient()
            .AddModrinthStandardResilienceHandler();

        return services;
    }
}

file static class Extensions
{
    public static IHttpClientBuilder ConfigureModrinthHttpClient(this IHttpClientBuilder builder)
    {
        builder.ConfigureHttpClient(
            (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ModrinthClientOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
                if (!string.IsNullOrEmpty(options.Token))
                    client.DefaultRequestHeaders.Add("Authorization", options.Token);
            }
        );
        return builder;
    }

    public static void AddModrinthStandardResilienceHandler(this IHttpClientBuilder builder)
    {
        var optionsName = $"{builder.Name}-standard";
        builder.AddResilienceHandler(
            "standard",
            (pipelineBuilder, context) =>
            {
                var resilienceOptions = context
                    .ServiceProvider.GetRequiredService<
                        IOptionsMonitor<HttpStandardResilienceOptions>
                    >()
                    .Get(optionsName);
                var modrinthClientOptions = context
                    .ServiceProvider.GetRequiredService<IOptions<ModrinthClientOptions>>()
                    .Value;
                pipelineBuilder
                    .AddRateLimiter(
                        new HttpRateLimiterStrategyOptions
                        {
                            DefaultRateLimiterOptions = new ConcurrencyLimiterOptions
                            {
                                PermitLimit = modrinthClientOptions.MaxConcurrentRequests,
                            },
                        }
                    )
                    .AddTimeout(resilienceOptions.TotalRequestTimeout)
                    .AddRetry(
                        new RetryStrategyOptions<HttpResponseMessage>
                        {
                            UseJitter = true,
                            BackoffType = DelayBackoffType.Exponential,
                            MaxRetryAttempts = modrinthClientOptions.RateLimitRetryCount,
                            ShouldHandle = args =>
                                ValueTask.FromResult(
                                    args.Outcome.Result?.StatusCode
                                        == HttpStatusCode.TooManyRequests
                                ),
                            DelayGenerator = args =>
                            {
                                try
                                {
                                    var response = args.Outcome.Result;
                                    if (
                                        response?.Headers.TryGetValues(
                                            "X-RateLimit-Reset",
                                            out var values
                                        ) == true
                                        && long.TryParse(values.FirstOrDefault(), out var resetUnix)
                                    )
                                    {
                                        var resetTime = DateTimeOffset.FromUnixTimeSeconds(
                                            resetUnix
                                        );
                                        var delay = resetTime - DateTimeOffset.UtcNow;

                                        if (delay > TimeSpan.Zero)
                                            return ValueTask.FromResult<TimeSpan?>(delay);
                                    }

                                    return ValueTask.FromResult<TimeSpan?>(
                                        TimeSpan.FromSeconds(Math.Pow(2, args.AttemptNumber))
                                    );
                                }
                                catch (Exception exception)
                                {
                                    return ValueTask.FromException<TimeSpan?>(exception);
                                }
                            },
                        }
                    )
                    .AddCircuitBreaker(resilienceOptions.CircuitBreaker)
                    .AddTimeout(resilienceOptions.AttemptTimeout);
            }
        );
        builder.ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan);
    }
}
