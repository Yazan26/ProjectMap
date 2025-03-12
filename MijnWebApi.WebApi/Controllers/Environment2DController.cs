using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MijnWebApi.WebApi.Classes.Models;


[ApiController]
[Route("[controller]")]
public class Environment2DController : ControllerBase
{
    private readonly IEnvironment2DRepository _environment2DRepository;
    private readonly ILogger<Environment2DController> _logger;
    private readonly IActionDescriptorCollectionProvider _actionDescriptorProvider;

    public Environment2DController(
        IEnvironment2DRepository environment2DRepository,
        ILogger<Environment2DController> logger,
        IActionDescriptorCollectionProvider actionDescriptorProvider)
    {
        _environment2DRepository = environment2DRepository;
        _logger = logger;
        _actionDescriptorProvider = actionDescriptorProvider;
    }

    [HttpGet]
    public async Task<IEnumerable<Environment2D>> Get()
    {
        return await _environment2DRepository.GetAllEnvironment2DsAsync();
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<Environment2D>> GetById(Guid id)
    {
        var world = await _environment2DRepository.GetWorldByIdAsync(id);
        if (world == null)
        {
            return NotFound();
        }
        return Ok(world);
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Environment2D>>> GetUserWorlds(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("Invalid user ID.");
        }

        var userWorlds = await _environment2DRepository.GetWorldsByUserIdAsync(userId);

        if (userWorlds == null || !userWorlds.Any())
        {
            return NotFound("No worlds found for this user.");
        }

        return Ok(userWorlds);
    }



    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Environment2D environment2D)
    {
        try
        { 
            // Ensure the GUID is valid
            
            if(environment2D == null)
            {
                return BadRequest("Invalid request: Environment2D is null!");
            }
            
            if (environment2D.OwnerUserID == Guid.Empty)
            {
                _logger.LogError("❌ ERROR: Invalid or missing AppUserId.");
                return BadRequest("Invalid or missing AppUserId.");
            }

            // ✅ Assign a new GUID if the Id is missing
            if (environment2D.Id == Guid.Empty)
            {
                environment2D.Id = Guid.NewGuid();
            }

            await _environment2DRepository.AddWorldAsync(environment2D);

         

            return CreatedAtAction(nameof(GetById), new { id = environment2D.Id }, environment2D);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error"+ ex.Message);
        }
    }





    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] Environment2D environment2D)
    {
        if (environment2D == null || id != environment2D.Id)
        {
            return BadRequest("Invalid world data.");
        }

        await _environment2DRepository.UpdateWorldAsync(environment2D);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _environment2DRepository.DeleteWorldAsync(id);
        return NoContent();
    }

    [HttpGet("world/{id:guid}")]
    public async Task<ActionResult<Environment2D>> GetSingleWorld(Guid id)
    {
        var world = await _environment2DRepository.GetWorldByIdAsync(id);
        if (world == null)
        {
            return NotFound($"❌ No world found with ID: {id}");
        }
        return Ok(world);
    }



    [HttpGet("objects/{WorldId}")]
    [Authorize]
    public async Task<ActionResult<Guid>> GetObjects(Guid WorldId)
    {
        _logger.LogInformation("Fetching worlds for user with Id: {UserId}", WorldId);
        var userWorlds = await _environment2DRepository.GetObjectsForWorld(WorldId);
        if (userWorlds == null)
        {

            return NotFound();
        }
        _logger.LogInformation("Worlds found: {userWorlds}", userWorlds);
        return Ok(userWorlds);
    }
}