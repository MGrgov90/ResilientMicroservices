using Microsoft.AspNetCore.Mvc;

namespace ApiRetry.Controllers;

[ApiController]
[Route("[controller]")]
public class YetAnotherProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public YetAnotherProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("YetAnotherProxyHttpClient");
    }

    [HttpGet]
    public async Task<User> GetUser()
    {
        return await _httpClient.GetFromJsonAsync<User>("Users/GetUserWithBreakingOnException");
    }
}
