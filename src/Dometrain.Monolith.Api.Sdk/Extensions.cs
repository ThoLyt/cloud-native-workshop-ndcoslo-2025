using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Refit;

namespace Dometrain.Monolith.Api.Sdk;

public static class Extensions
{
    public static IServiceCollection AddDometrainApi(this IServiceCollection services,
        string baseUrl, string apiKey)
    {
        services.AddRefitClient<IStudentsApiClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Add("x-api-key", apiKey);
            }).AddResilienceHandler("simple-fancy-retry", builder =>
            {
                builder
                    .AddTimeout(TimeSpan.FromMinutes(1))
                    .AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = static args => ValueTask.FromResult(args is
                    {
                        Outcome.Result.StatusCode: 
                        HttpStatusCode.RequestTimeout or 
                        HttpStatusCode.TooManyRequests or 
                        HttpStatusCode.ServiceUnavailable
                    }),
                    OnRetry = arguments =>
                    {
                        return ValueTask.CompletedTask;
                    }
                });
            });
        
        
        
        services.AddRefitClient<ICoursesApiClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Add("x-api-key", apiKey);
            });
        return services;
    }
}
