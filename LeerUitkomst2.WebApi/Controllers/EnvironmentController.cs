using LeerUitkomst2.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectMap.WebApi.Repositories;

namespace LeerUitkomst2.WebApi.Controllers;

[Authorize]
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
    private readonly IAuthenticationService _authService;

    public EnvironmentController(EnvironmentRepository weatherForecastRepository, 
        ILogger<EnvironmentController> logger,
        IAuthenticationService authService)
    {
        _environment2DRepository = weatherForecastRepository;
        _logger = logger;
        _authService = authService;
    }

    [HttpGet(Name = "ReadEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        var env2D = await _environment2DRepository.ReadAsync();
        return Ok(env2D);
        // this._authService.GetCurrentAuthenticatedUserId();
    }

    [HttpGet("{environmentId}", Name = "ReadEnvironment")]
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

    [HttpPut(Name = "UpdateEnvironment")]
    public async Task<ActionResult> Update(Environment2D environment)
    {
        var existingEnvironment = await _environment2DRepository.ReadAsync(environment.Id);

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


