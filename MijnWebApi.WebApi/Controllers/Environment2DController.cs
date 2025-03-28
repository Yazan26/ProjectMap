using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MijnWebApi.WebApi.Classes.Models;

[ApiController]
[Route("[controller]")]
[Authorize]
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
    /// Gets all Environment2D records for the current authenticated user.
    /// </summary>
    /// <returns>A list of Environment2D records.</returns>
    /// <remarks>
    /// Route: GET /Environment2D
    /// </remarks>
    [HttpGet(Name = "GetWorlds")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        Guid CurrentUser = Guid.Parse(_authenticationService.GetCurrentAuthenticatedUserId());
        var Worlds = await _environment2DRepository.GetWorldsForUserAsync(CurrentUser);
        return Ok(Worlds);
    }

    /// <summary>
    /// Gets an Environment2D record by ID.
    /// </summary>
    /// <param name="Environment2DId">The ID of the Environment2D record.</param>
    /// <returns>The Environment2D record.</returns>
    /// <remarks>
    /// Route: GET /Environment2D/{Environment2DId}
    /// </remarks>
    [HttpGet("{Environment2DId}", Name = "GetWorld")]
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
    /// Creates a new Environment2D record.
    /// </summary>
    /// <param name="World">The Environment2D record to create.</param>
    /// <returns>Action result.</returns>
    /// <remarks>
    /// Route: POST /Environment2D/CreateWorld
    /// Example JSON body:
    /// {
    ///     "Name": "New World",
    ///     "MaxHeight": 100,
    ///     "MaxWidth": 100
    /// }
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

    /// <summary>
    /// Updates an existing Environment2D record.
    /// </summary>
    /// <param name="Environment2DId">The ID of the Environment2D record to update.</param>
    /// <param name="NewWorld">The updated Environment2D record.</param>
    /// <returns>Action result.</returns>
    /// <remarks>
    /// Route: PUT /Environment2D/{Environment2DId}
    /// Example JSON body:
    /// {
    ///     "Id": "guid",
    ///     "Name": "Updated World",
    ///     "MaxHeight": 200,
    ///     "MaxWidth": 200,
    ///     "OwnerUserID": "guid"
    /// }
    /// </remarks>
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
    /// <param name="WorldId">The ID of the Environment2D record to delete.</param>
    /// <returns>Action result.</returns>
    /// <remarks>
    /// Route: DELETE /Environment2D/{WorldId}
    /// </remarks>
    [HttpDelete("{Environment2DId}", Name = "DeleteWorld")]
    public async Task<ActionResult> Delete(Guid WorldId)
    {
        var ExistingWorld = await _environment2DRepository.GetWorldAsync(WorldId);
        if (ExistingWorld == null)
            return NotFound();

        await _environment2DRepository.DeleteWorldAsync(WorldId);
        return Ok();
    }
}
