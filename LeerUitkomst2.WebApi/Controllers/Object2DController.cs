using LeerUitkomst2.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ProjectMap.WebApi.Repositories;
using System;

namespace LeerUitkomst2.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class Object2DController : ControllerBase
{
    private readonly EnvironmentRepository _environment2DRepository;
    private readonly Object2DRepository _object2DRepository;
    private readonly ILogger<EnvironmentController> _logger;
    private readonly IAuthenticationService _authService;

    public Object2DController(EnvironmentRepository environmentRepository,
        Object2DRepository objectRepository,
        ILogger<EnvironmentController> logger,
        IAuthenticationService authService)
    {
        _environment2DRepository = environmentRepository;
        _logger = logger;
        _authService = authService;
        _object2DRepository = objectRepository;
    }

    [HttpPost(Name = "CreateObject")]
    public async Task<ActionResult> Add(Object2DTemplate template)
    {
        string userId = this._authService.GetCurrentAuthenticatedUserId();
        if (userId == null)
        {
            return BadRequest();
        }
        DatabaseBundle<Environment2D> dataBundle = new DatabaseBundle<Environment2D>()
        {
            UserId = userId,
            DatabaseRepository = _environment2DRepository,
            RequestedId = template.EnvironmentId
        };
        var authResponse = await Validator.Environment2DAccessCheck(dataBundle);
        this._logger.LogInformation($"User '{userId}' tried to access '{template.EnvironmentId}': '{authResponse.LoggerMessage}'");
        if (authResponse.Value == false)
        {
            return BadRequest(authResponse.Message);
        }
        var result = await this._object2DRepository.CreateObject(template);
        return Ok(result);

    }


    //[HttpGet(Name = "ReadEnvironments")]
    //public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    //{
    //    var env2D = await _environment2DRepository.ReadAsync();
    //    return Ok(env2D);
    //    // this._authService.GetCurrentAuthenticatedUserId();
    //}

    //[HttpGet("{environmentId}", Name = "ReadEnvironment")]
    //public async Task<ActionResult<Environment2D>> Get(int environmentId)
    //{
    //    var environment = await _environment2DRepository.ReadAsync(environmentId);
    //    if (environment == null)
    //        return NotFound();

    //    return Ok(environment);
    //}

    //[HttpPost(Name = "CreateEnvironment")]
    //public async Task<ActionResult> Add(Environment2D environment)
    //{
    //    var createdEnvironment = await _environment2DRepository.InsertAsync(environment);
    //    return Created();
    //}

    //[HttpPut(Name = "UpdateEnvironment")]
    //public async Task<ActionResult> Update(Environment2D environment)
    //{
    //    var existingEnvironment = await _environment2DRepository.ReadAsync(environment.Id);

    //    if (existingEnvironment == null)
    //        return NotFound();

    //    await _environment2DRepository.UpdateAsync(environment);

    //    return Ok(environment);
    //}

    //[HttpDelete("{environmentId}", Name = "DeleteEnvironmentById")]
    //public async Task<IActionResult> Update(int environmentId)
    //{
    //    var existingEnvironment = await _environment2DRepository.ReadAsync(environmentId);

    //    if (existingEnvironment == null)
    //        return NotFound();

    //    await _environment2DRepository.DeleteAsync(environmentId);

    //    return Ok();
    //}
}


