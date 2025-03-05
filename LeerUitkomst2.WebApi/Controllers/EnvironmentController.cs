using LeerUitkomst2.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectMap.WebApi.Repositories;
using System;

namespace LeerUitkomst2.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EnvironmentController : ControllerBase
{
    private readonly EnvironmentRepository _environment2DRepository;
    private readonly Object2DRepository _object2DRepository;
    private readonly ILogger<EnvironmentController> _logger;
    private readonly IAuthenticationService _authService;

    public EnvironmentController(EnvironmentRepository environmentRepository,
        Object2DRepository objectRepository,
        ILogger<EnvironmentController> logger,
        IAuthenticationService authService)
    {
        _environment2DRepository = environmentRepository;
        _logger = logger;
        _authService = authService;
        _object2DRepository = objectRepository;
    }

    [HttpPost(Name = "CreateEnvironment")]
    public async Task<ActionResult> Add(Environment2DTemplate environment)
    {
        string userId = this._authService.GetCurrentAuthenticatedUserId();
        if (environment == null || userId == null)
        {
            return BadRequest();
        }
        var amountOfEnvironments = await _environment2DRepository.GetAmountByUser(userId);
        var environmentsNameList = await this._environment2DRepository.FindEnvironmentByName(userId, environment.Name);
        DataBoolean allowedOrNot = Validator.AllowedToCreateEnvironment(environment, amountOfEnvironments, (List<Environment2D>)environmentsNameList);
        this._logger.LogInformation($"User '{userId}' tried to create an environment: '{allowedOrNot.LoggerMessage}'");
        if (allowedOrNot.Value == false)
        {
            return BadRequest(allowedOrNot.Message);
        }
        var createdEnvironment = await _environment2DRepository.CreateEnvironmentByUser(environment, userId);
        return Ok(createdEnvironment);
    }

    [HttpGet(Name = "GetEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        string userId = this._authService.GetCurrentAuthenticatedUserId();
        if (userId == null)
        {
            return BadRequest();
        }
        var environmentList = await _environment2DRepository.GetEnvironmentByUser(userId);
        return Ok(environmentList);
    }

    [HttpGet("{environmentId}", Name = "GetEnvironmentData")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get(int environmentId)
    {
        string userId = this._authService.GetCurrentAuthenticatedUserId();
        if (userId == null)
        {
            return BadRequest();
        }
        DatabaseBundle dataBundle = new DatabaseBundle()
        {
            UserId = userId,
            DatabaseRepository = _environment2DRepository,
            RequestedId = environmentId
        };
        var authResponse = await Validator.Environment2DAccessCheck(dataBundle);
        this._logger.LogInformation($"User '{userId}' tried to open environment '{environmentId}': '{authResponse.LoggerMessage}'");
        if (authResponse.Value == false)
        {
            return BadRequest(authResponse.Message);
        }
        var objects = await this._object2DRepository.GetAllByEnvironmentId(environmentId);
        var response = new
        {
            EnvironmentId = environmentId,
            EnvironmentData = authResponse.ExtraData,
            EnvironmentObjects = objects
        };
        return Ok(response);
    }

    [HttpDelete("{environmentId}", Name = "DeleteEnvironmentById")]
    public async Task<IActionResult> Update(int environmentId)
    {
        string userId = this._authService.GetCurrentAuthenticatedUserId();
        if (userId == null)
        {
            return BadRequest();
        }
        DatabaseBundle dataBundle = new DatabaseBundle()
        {
            UserId = userId,
            DatabaseRepository = _environment2DRepository,
            RequestedId = environmentId
        };
        var authResponse = await Validator.Environment2DAccessCheck(dataBundle);
        this._logger.LogInformation($"User '{userId}' tried to delete '{environmentId}': '{authResponse.LoggerMessage}'");
        if (authResponse.Value == false)
        {
            return BadRequest(authResponse.Message);
        }
        var deleteSuccess = await _environment2DRepository.DeleteAsync(environmentId);
        this._logger.LogInformation($"User '{userId}' was authorized to delete '{environmentId}': '{deleteSuccess.LoggerMessage}'");
        if (deleteSuccess.Value == false)
        {
            return BadRequest(deleteSuccess.Message);
        }
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


