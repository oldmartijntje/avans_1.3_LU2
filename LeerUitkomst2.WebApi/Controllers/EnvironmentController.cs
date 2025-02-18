using LeerUitkomst2.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public ActionResult<IEnumerable<Environment2D>> GetAll()
    {
        // db select
        return Ok(_testData);
    }

    [HttpGet("{id}")]
    public ActionResult<Environment2D> GetById(int id)
    {
        // db select specifstick
        var environment = _testData.Find(e => e.Id == id);
        if (environment == null)
            return NotFound();
        return Ok(environment);
    }

    [HttpPost]
    public ActionResult<Environment2D> Create(Environment2D environment)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        // db create
        environment.Id = _testData.Count + 1;
        _testData.Add(environment);
            
        return CreatedAtAction(nameof(GetById), new { id = environment.Id }, environment);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Environment2D environment)
    {
        if (id != environment.Id || !ModelState.IsValid)
            return BadRequest();
            
        // db put
        var existing = _testData.Find(e => e.Id == id);
        if (existing == null)
            return NotFound();
            
        existing.Name = environment.Name;
        existing.MaxHeight = environment.MaxHeight;
        existing.MaxLength = environment.MaxLength;
            
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // db delete
        var environment = _testData.Find(e => e.Id == id);
        if (environment == null)
            return NotFound();
            
        _testData.Remove(environment);
        return NoContent();
    }
}


