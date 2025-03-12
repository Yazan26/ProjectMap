using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MijnWebApi.WebApi.Classes.Models;


[ApiController]
[Route("[controller]")]
public class Object2DController : ControllerBase
{
    private readonly IObject2DRepository _Object2DRepository;
    private readonly ILogger<Object2DController> _logger;

    public Object2DController(IObject2DRepository Object2DRepository, ILogger<Object2DController> logger)
    {
        _Object2DRepository = Object2DRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Object2D>> Get()
    {
        _logger.LogInformation("Fetching all game objects.");
        return await _Object2DRepository.GetAllObject2DsAsync();
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<Object2D>> GetById(Guid id)
    {
        _logger.LogInformation($"Fetching game object with ID: {id}");
        var Object2D = await _Object2DRepository.GetObject2DByIdAsync(id);
        if (Object2D == null)
        {
            _logger.LogWarning($"Game object with ID {id} not found.");
            return NotFound();
        }
        return Ok(Object2D);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Object2D Object2D)
    {
        if (Object2D == null)
        {
            _logger.LogWarning("Attempted to create a null game object.");
            return BadRequest("Invalid game object data.");
        }

        Object2D.Id = Object2D.Id == Guid.Empty ? Guid.NewGuid() : Object2D.Id;
        _logger.LogInformation($"Creating game object with ID: {Object2D.Id}");

        await _Object2DRepository.AddObject2DAsync(Object2D);
        return CreatedAtAction(nameof(GetById), new { id = Object2D.Id }, Object2D);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] Object2D Object2D)
    {
        if (Object2D == null || id != Object2D.Id)
        {
            _logger.LogWarning("Game object ID mismatch or invalid data provided.");
            return BadRequest("Invalid game object data.");
        }

        _logger.LogInformation($"Updating game object with ID: {id}");
        await _Object2DRepository.UpdateObject2DAsync(Object2D);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation($"Deleting game object with ID: {id}");
        await _Object2DRepository.DeleteObject2DAsync(id);
        return NoContent();
    }
}
