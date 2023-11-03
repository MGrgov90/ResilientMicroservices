using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ThirdPartyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IMemoryCache _cache;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        _cache = memoryCache;
    }

    [HttpGet]
    public IEnumerable<User> Get()
    {
        return GenFu.GenFu.ListOf<User>(5);
    }

    [HttpGet]
    [Route("GetUser")]
    public User GetUser()
    {
        return GenFu.GenFu.New<User>();
    }

    [HttpGet]
    [Route("GetUserRandomly")]
    public User GetUserRandomly()
    {
        var rnd = new Random().Next(10);
        if (rnd <= 5)
            throw new Exception("Service unavailable");
        else return GenFu.GenFu.New<User>();
    }

    [HttpGet]
    [Route("GetUserWithBreakingOnException")]
    public User GetUserWithBreakingOnException()
    {
        var rnd = new Random().Next(10);
        if (rnd <= 7)
            throw new Exception("Service unavailable");
        else return GenFu.GenFu.New<User>();
    }
}