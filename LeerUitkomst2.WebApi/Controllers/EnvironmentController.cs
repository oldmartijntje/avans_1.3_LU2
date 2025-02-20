using LeerUitkomst2.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using ProjectMap.WebApi.Repositories;

namespace LeerUitkomst2.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnvironmentController : ControllerBase
{
    private static readonly List<Environment2D> _testData = new()
    {
        new Environment2D { Id = 1, Name = "Env1", MaxHeight = 50, MaxLength = 100 },
        new Environment2D { Id = 2, Name = "Env2", MaxHeight = 75, MaxLength = 150 }
    };
    private readonly EnvironmentRepository _environment2DRepository;
    private readonly ILogger<EnvironmentController> _logger;

    public EnvironmentController(EnvironmentRepository weatherForecastRepository, ILogger<EnvironmentController> logger)
    {
        _environment2DRepository = weatherForecastRepository;
        _logger = logger;
    }

    [HttpGet(Name = "ReadEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        var env2D = await _environment2DRepository.ReadAsync();
        return Ok(env2D);
    }

    [HttpGet("{EnvironmentId}", Name = "ReadEnvironment")]
    public async Task<ActionResult<Environment2D>> Get(int environmentId)
    {
        var environment = await _environment2DRepository.ReadAsync(environmentId);
        if (environment == null)
            return NotFound();

        return Ok(environment);
    }

    [HttpPost(Name = "CreateEnvironment")]
    public async Task<ActionResult> Add(Environment2D environment)
    {
        var createdEnvironment = await _environment2DRepository.InsertAsync(environment);
        return Created();
    }

    [HttpPut("{environmentId}", Name = "UpdateEnvironment")]
    public async Task<ActionResult> Update(int environmentId, Environment2D environment)
    {
        var existingEnvironment = await _environment2DRepository.ReadAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound();

        await _environment2DRepository.UpdateAsync(environment);

        return Ok(environment);
    }

    [HttpDelete("{environmentId}", Name = "DeleteEnvironmentById")]
    public async Task<IActionResult> Update(int environmentId)
    {
        var existingEnvironment = await _environment2DRepository.ReadAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound();

        await _environment2DRepository.DeleteAsync(environmentId);

        return Ok();
    }
}


