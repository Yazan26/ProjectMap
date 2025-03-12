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
    private readonly IAuthenticationService _authenticationService; // ✅ New Authentication Service
    private readonly ILogger<Object2DController> _logger;

    public Object2DController(
        IObject2DRepository Object2DRepository,
        IAuthenticationService authenticationService, // ✅ Inject Authentication Service
        ILogger<Object2DController> logger)
    {
        _Object2DRepository = Object2DRepository;
        _authenticationService = authenticationService; // ✅ Save Auth Service
        _logger = logger;
    }

    [HttpGet("user/world/{worldId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Object2D>>> GetObjectsForUserWorld(Guid worldId)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User not authenticated.");
            }

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var objects = await _Object2DRepository.GetObjectsForUserWorld(userId, worldId);

            if (objects == null || !objects.Any())
            {
                return NotFound("No objects found for this user in the selected world.");
            }

            return Ok(objects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
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

        var userId = _authenticationService.GetCurrentAuthenticatedUserId(); // ✅ Ensure User is Authenticated
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        Object2D.Id = Object2D.Id == Guid.Empty ? Guid.NewGuid() : Object2D.Id;
        _logger.LogInformation($"📡 Creating game object with ID: {Object2D.Id} for UserID: {userId}");

        await _Object2DRepository.AddObject2DAsync(Object2D);
        return CreatedAtAction(nameof(GetObjectsForUserWorld), new { worldId = Object2D.Environment2DID }, Object2D);
    }
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteObject2D(Guid id)
    {
        var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest("Invalid user ID.");
        }

        _logger.LogInformation($"🗑️ Deleting Object {id} for User {userId}");

        var deleted = await _Object2DRepository.DeleteObject2DAsync(id, userId);
        if (deleted)
        {
            return NoContent();
        }
        else
        {
            return NotFound("Object not found or you don't have permission to delete it.");
        }
    }

}
