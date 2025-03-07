using LeerUitkomst2.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectMap.WebApi.Repositories;

namespace LeerUitkomst2.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class Object2DController : ControllerBase
{
    private readonly EnvironmentRepository _environment2DRepository;
    private readonly Object2DRepository _object2DRepository;
    private readonly ILogger<Object2DController> _logger;
    private readonly IAuthenticationService _authService;

    public Object2DController(EnvironmentRepository environmentRepository,
        Object2DRepository objectRepository,
        ILogger<Object2DController> logger,
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
        string? userId = this._authService.GetCurrentAuthenticatedUserId();
        if (userId == null)
        {
            return BadRequest();
        }
        template.FixRotation();
        DatabaseBundle dataBundle = new DatabaseBundle()
        {
            UserId = userId,
            DatabaseRepository = _environment2DRepository,
            RequestedId = template.EnvironmentId
        };
        var authResponse = await Validator.Environment2DAccessCheck(dataBundle);
        this._logger.LogInformation($"User '{userId}' tried to access env '{template.EnvironmentId}': '{authResponse.LoggerMessage}'");
        if (authResponse.Value == false)
        {
            return BadRequest(authResponse.Message);
        }
        var validObjectResponse = Validator.IsValidObject2D(template, authResponse.ExtraData as Environment2D);
        this._logger.LogInformation($"User '{userId}' tried to create object: '{validObjectResponse.LoggerMessage}'");
        if (validObjectResponse.Value == false)
        {
            return BadRequest(validObjectResponse.Message);
        }
        var result = await this._object2DRepository.CreateObject(template);
        return Ok(result);

    }

    [HttpPut(Name = "EditObject")]
    public async Task<ActionResult> Update(Object2D template)
    {
        string? userId = this._authService.GetCurrentAuthenticatedUserId();
        if (userId == null)
        {
            return BadRequest();
        }
        template.FixRotation();
        DatabaseBundle dataBundle = new DatabaseBundle()
        {
            UserId = userId,
            DatabaseRepository = _object2DRepository,
            RequestedId = template.Id
        };
        var authResponse = await Validator.Environment2DAccessCheck(dataBundle);
        this._logger.LogInformation($"User '{userId}' tried to access env '{template.EnvironmentId}': '{authResponse.LoggerMessage}'");
        if (authResponse.Value == false)
        {
            return BadRequest(authResponse.Message);
        }
        Environment2D? realEnvironment = authResponse.ExtraData as Environment2D;
        if (realEnvironment == null)
        {
            return BadRequest("Environment = null");
        }
        template.EnvironmentId = realEnvironment.Id;
        var validObjectResponse = Validator.IsValidObject2D(template, realEnvironment);
        this._logger.LogInformation($"User '{userId}' tried to edit object: '{validObjectResponse.LoggerMessage}'");
        if (validObjectResponse.Value == false)
        {
            return BadRequest(validObjectResponse.Message);
        }
        await this._object2DRepository.UpdateAsync(template);
        return Ok();

    }
    
    [HttpDelete("{objectId}", Name = "DeleteObjectById")]
    public async Task<IActionResult> Remove(int objectId)
    {
        string? userId = this._authService.GetCurrentAuthenticatedUserId();
        DatabaseBundle dataBundle = new DatabaseBundle()
        {
            UserId = userId,
            DatabaseRepository = _object2DRepository,
            RequestedId = objectId
        };
        var authResponse = await Validator.Environment2DAccessCheck(dataBundle);
        this._logger.LogInformation($"User '{userId}' tried to delete obj '{objectId}': '{authResponse.LoggerMessage}'");
        if (authResponse.Value == false)
        {
            return BadRequest(authResponse.Message);
        }
        var existingObject = await _object2DRepository.ReadAsync(objectId);

        if (existingObject == null)
            return NotFound();

        await _object2DRepository.DeleteAsync(objectId);

        return Ok();
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


