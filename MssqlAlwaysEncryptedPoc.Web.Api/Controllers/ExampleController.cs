using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MssqlAlwaysEncryptedPoc.ExampleDb;

namespace MssqlAlwaysEncryptedPoc.Web.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;
    private readonly ExampleDbContext _dbContext;

    public ExampleController(ILogger<ExampleController> logger, ExampleDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet(Name = "GetExamples")]
    public async Task<IEnumerable<Example>> Get()
    {
        return await _dbContext.Examples.ToListAsync();
    }
}
