using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace ApiRetry.Helpers;

public static class PolicyHelpers
{
    internal static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    {
        IAsyncPolicy<HttpResponseMessage> httpWaitAndRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                        (result, span, retryCount, ctx) => Console.WriteLine($"Retrying({retryCount})...")
                    );
        IAsyncPolicy<HttpResponseMessage> fallbackPolicy =
            Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .FallbackAsync(FallbackAction, OnFallbackAsync);

        return Policy.WrapAsync(fallbackPolicy, httpWaitAndRetryPolicy);
    }

    internal static Task OnFallbackAsync(DelegateResult<HttpResponseMessage> response,
        Context context)
    {
        Console.WriteLine("About to call the fallback action. This is a good place to do some logging");
        return Task.CompletedTask;
    }

    internal static Task<HttpResponseMessage> FallbackAction(DelegateResult<HttpResponseMessage> responseToFailedRequest,
        Context context, CancellationToken cancellationToken)
    {
        Console.WriteLine("Fallback action is executing...");

        Console.WriteLine("\n\tSending message to admin\n");
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(responseToFailedRequest.Result.StatusCode)
        {
            Content = new StringContent($"The fallback executed, the original error was {responseToFailedRequest.Result.ReasonPhrase}")
        };
        return Task.FromResult(httpResponseMessage);
    }

    internal static IAsyncPolicy<HttpResponseMessage> GetAdvancedPolicy()
    {
        var waitAndRetryPolicy =
                HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(
                        medianFirstRetryDelay: TimeSpan.FromSeconds(1),
                        retryCount: 3));

        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(4,
                TimeSpan.FromSeconds(60));

        return Policy.WrapAsync(waitAndRetryPolicy, circuitBreakerPolicy);
    }
}
