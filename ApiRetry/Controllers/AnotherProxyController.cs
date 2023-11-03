using Microsoft.AspNetCore.Mvc;

namespace ApiRetry.Controllers;

[ApiController]
[Route("[controller]")]
public class AnotherProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public AnotherProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AnotherProxyHttpClient");
    }

    [HttpGet]
    public async Task<User> GetUser()
    {
        return await _httpClient.GetFromJsonAsync<User>("Users/GetUserRandomly");
    }
}
