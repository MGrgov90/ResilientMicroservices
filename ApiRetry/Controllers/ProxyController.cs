using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;

namespace ApiRetry.Controllers;

[ApiController]
[Route("[controller]")]
public class ProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly AsyncFallbackPolicy<IActionResult> _fallbackPolicy;
    private readonly AsyncRetryPolicy<IActionResult> _retryPolicy;

    private static AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public ProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();

        _fallbackPolicy = Policy<IActionResult>
                .Handle<Exception>()
                .FallbackAsync(Content("Sorry, we are currently experiencing issues. Please try again later"));

        _retryPolicy = Policy<IActionResult>
            .Handle<Exception>()
            .RetryAsync(5);

        if (_circuitBreakerPolicy == null)
        {
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
        }

    }


    [HttpGet]
    public async Task<User> User() =>
        await _httpClient.GetFromJsonAsync<User>("http://localhost:5153/Users/GetUser");

    [HttpGet]
    [Route("UserRandomly")]
    public async Task<User> UserRandomly() =>
        await _httpClient.GetFromJsonAsync<User>("http://localhost:5153/Users/GetUserRandomly");

    [HttpGet]
    [Route("UserRandomlyWithFallback")]
    public async Task<IActionResult> UserRandomlyWithFallback() =>
        await _fallbackPolicy.ExecuteAsync(async ()
            => Content(await _httpClient.GetStringAsync("http://localhost:5153/Users/GetUserRandomly")));

    [HttpGet]
    [Route("UserRandomlyWithRetry")]
    public async Task<IActionResult> UserRandomlyWithRetry() =>
        await _retryPolicy.ExecuteAsync(async ()
            => Content(await _httpClient.GetStringAsync("http://localhost:5153/Users/GetUserRandomly")));

    [HttpGet]
    [Route("UserRandomlyWithBreakingCircuit")]
    public async Task<IActionResult> UserRandomlyWithBreakingCircuit() =>
        await _circuitBreakerPolicy.ExecuteAsync(async ()
            => Content(await _httpClient.GetStringAsync("http://localhost:5153/Users/GetUserWithBreakingOnException")));
}
