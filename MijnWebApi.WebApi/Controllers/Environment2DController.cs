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
    private readonly IAuthenticationService _authenticationService;
 

    public Environment2DController(
        IEnvironment2DRepository environment2DRepository,
        ILogger<Environment2DController> logger,
        IAuthenticationService authenticationService)
    {
        _environment2DRepository = environment2DRepository;
        _logger = logger;
        _authenticationService = authenticationService;
    }


    /// <summary>
    /// Gets all Environment2D records.
    /// </summary>
    /// <returns>A list of Environment2D records.</returns>
    /// <remarks>
    /// Route: GET /Environment2D
    /// </remarks>
    [HttpGet(Name = "GetWorlds")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        Guid CurrentUser = Guid.Parse(_authenticationService.GetCurrentAuthenticatedUserId());
        var Worlds = await _environment2DRepository.GetWorldAsync(CurrentUser);
        return Ok(Worlds);
    }


    /// <summary>
    /// Gets an Environment2D record by ID.
    /// </summary>
    /// <param name="id">The ID of the Environment2D record.</param>
    /// <returns>The Environment2D record.</returns>
    /// <remarks>
    /// Route: GET /Environment2D/{id}
    /// </remarks>
    [HttpGet("{Environment2DId}\", Name = \"GetWorld")]
    public async Task<ActionResult<Environment2D>> Get(Guid Environment2DId)
    {
        var world = await _environment2DRepository.GetWorldAsync(Environment2DId);
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
    [HttpPost("CreateWorld")]
    public async Task<ActionResult> Add(Environment2D World)
    {
        Guid CurrentUser = Guid.Parse(_authenticationService.GetCurrentAuthenticatedUserId());
        var worlds = await _environment2DRepository.GetWorldsForUserAsync(CurrentUser);
        bool NameExists = false;
        foreach (Environment2D NameTest in worlds)
        {
            if (NameTest.Name == World.Name)
            {
                NameExists = true;
                return BadRequest();
            }
        }
        if (worlds.Count() >= 6 || NameExists)
        {
            return BadRequest();
        }
        else
        {
            World.Id = Guid.NewGuid();

            World.OwnerUserID = CurrentUser;

            var CreatedWorld = await _environment2DRepository.PostWorldAsync(World);
            return Ok();
        }

    }

    [HttpPut("{Environment2DId}", Name = "UpdateWorld")]
    public async Task<ActionResult> Update(Guid Environment2DId, Environment2D NewWorld)
    {
        var ExistingWorld = await _environment2DRepository.GetWorldAsync(Environment2DId);

        if (ExistingWorld == null)
            return NotFound();

        await _environment2DRepository.UpdateWorldAsync(NewWorld);

        return Ok(NewWorld);
    }


    /// <summary>
    /// Deletes an Environment2D record by ID.
    /// </summary>
    /// <param name="id">The ID of the Environment2D record to delete.</param>
    /// <returns>No content.</returns>
    /// <remarks>
    /// Route: DELETE /Environment2D/{id}
    /// </remarks>
    [HttpDelete("{Environment2DId}", Name = "DeleteWorld")]
    public async Task<ActionResult> Update(Guid WorldId)
    {
        var ExistingWorld = await _environment2DRepository.GetWorldAsync(WorldId);

        if (ExistingWorld == null)
            return NotFound();

        await _environment2DRepository.DeleteWorldAsync(WorldId);

        return Ok();
    }
}
