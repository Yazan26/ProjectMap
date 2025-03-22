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

    /// <summary>
    /// Gets all Environment2D records.
    /// </summary>
    /// <returns>A list of Environment2D records.</returns>
    /// <remarks>
    /// Route: GET /Environment2D
    /// </remarks>
    [HttpGet]
    public async Task<IEnumerable<Environment2D>> Get()
    {
        return await _environment2DRepository.GetAllEnvironment2DsAsync();
    }

    /// <summary>
    /// Gets an Environment2D record by ID.
    /// </summary>
    /// <param name="id">The ID of the Environment2D record.</param>
    /// <returns>The Environment2D record.</returns>
    /// <remarks>
    /// Route: GET /Environment2D/{id}
    /// </remarks>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<Environment2D>> GetById(Guid id)
    {
        var world = await _environment2DRepository.GetWorldByIdAsync(id);
        if (world == null)
        {
            return NotFound();
        }
        return Ok(world);
    }

    /// <summary>
    /// Gets all Environment2D records for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of Environment2D records for the user.</returns>
    /// <remarks>
    /// Route: GET /Environment2D/user/{userId}
    /// </remarks>
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

    /// <summary>
    /// Creates a new Environment2D record.
    /// </summary>
    /// <param name="environment2D">The Environment2D record to create.</param>
    /// <returns>The created Environment2D record.</returns>
    /// <remarks>
    /// Route: POST /Environment2D
    /// 
    /// Sample request:
    /// 
    ///     POST /Environment2D
    ///     {
    ///         "name": "Sample World",
    ///         "maxHeight": 100,
    ///         "maxWidth": 100,
    ///         "ownerUserID": "d290f1ee-6c54-4b01-90e6-d701748f0851"
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Environment2D environment2D)
    {
        try
        {
            if (environment2D == null)
            {
                return BadRequest("Invalid request: Environment2D is null!");
            }

            if (environment2D.OwnerUserID == Guid.Empty)
            {
                _logger.LogError("❌ ERROR: Invalid or missing UserId.");
                return BadRequest("Invalid or missing UserId.");
            }

            if (environment2D.Id == Guid.Empty)
            {
                environment2D.Id = Guid.NewGuid();
            }

            await _environment2DRepository.AddWorldAsync(environment2D);

            return CreatedAtAction(nameof(GetById), new { id = environment2D.Id }, environment2D);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error" + ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing Environment2D record.
    /// </summary>
    /// <param name="id">The ID of the Environment2D record to update.</param>
    /// <param name="environment2D">The updated Environment2D record.</param>
    /// <returns>No content.</returns>
    /// <remarks>
    /// Route: PUT /Environment2D/{id}
    /// 
    /// Sample request:
    /// 
    ///     PUT /Environment2D/{id}
    ///     {
    ///         "id": "d290f1ee-6c54-4b01-90e6-d701748f0851",
    ///         "name": "Updated World",
    ///         "maxHeight": 200,
    ///         "maxWidth": 200,
    ///         "ownerUserID": "d290f1ee-6c54-4b01-90e6-d701748f0851"
    ///     }
    /// </remarks>
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

    /// <summary>
    /// Deletes an Environment2D record by ID.
    /// </summary>
    /// <param name="id">The ID of the Environment2D record to delete.</param>
    /// <returns>No content.</returns>
    /// <remarks>
    /// Route: DELETE /Environment2D/{id}
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _environment2DRepository.DeleteWorldAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Gets all Object2D records for a specific Environment2D.
    /// </summary>
    /// <param name="WorldId">The ID of the Environment2D.</param>
    /// <returns>A list of Object2D records for the Environment2D.</returns>
    /// <remarks>
    /// Route: GET /Environment2D/objects/{WorldId}
    /// </remarks>
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
