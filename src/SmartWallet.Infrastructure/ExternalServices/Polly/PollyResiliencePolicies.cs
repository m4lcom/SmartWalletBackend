using Polly;
using Polly.Extensions.Http;

namespace SmartWallet.Infrastructure.ExternalServices.Polly
{
    public static class PollyResiliencePolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ApiClientConfiguration config)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.BadRequest)
                .WaitAndRetryAsync(
                    config.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) + config.RetryAttemptInSeconds)
                );
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ApiClientConfiguration config)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.BadRequest)
                .CircuitBreakerAsync(
                    config.HandledEventsAllowedBeforeBreaking,
                    TimeSpan.FromSeconds(config.DurationOfBreakInSeconds)
                );
        }
    }
}
