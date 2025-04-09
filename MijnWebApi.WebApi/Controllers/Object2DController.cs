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
[Authorize]
public class Object2DController : ControllerBase
{
    private readonly IObject2DRepository _Object2DRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<Object2DController> _logger;

    public Object2DController(
        IObject2DRepository Object2DRepository,
        IAuthenticationService authenticationService,
        ILogger<Object2DController> logger)
    {
        _Object2DRepository = Object2DRepository;
        _authenticationService = authenticationService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all Object2D records.
    /// </summary>
    /// <returns>A list of Object2D records.</returns>
    /// <remarks>
    /// Route: GET /Object2D
    /// </remarks>
    [HttpGet(Name = "GetObjects")]
    public async Task<ActionResult<IEnumerable<Object2D>>> Get()
    {
        var Objects = await _Object2DRepository.GetAllObjectsAsync();
        return Ok(Objects);
    }

    /// <summary>
    /// Gets Object2D records by Environment2DId.
    /// </summary>
    /// <param name="Environment2DId">The ID of the Environment2D.</param>
    /// <returns>A list of Object2D records.</returns>
    /// <remarks>
    /// Route: GET /Object2D/{Environment2DId}
    /// </remarks>
    [HttpGet("{Environment2DId}", Name = "GetObjectWorld")]
    public async Task<ActionResult<IEnumerable<Object2D>>> Get(Guid Environment2DId)
    {
        var Object2D = await _Object2DRepository.GetObjectAsync(Environment2DId);
        if (Object2D == null)
            return NotFound();

        return Ok(Object2D);
    }

    /// <summary>
    /// Creates a new Object2D record.
    /// </summary>
    /// <param name="Object">The Object2D record to create.</param>
    /// <returns>The created Object2D record.</returns>
    /// <remarks>
    /// Route: POST /Object2D
    /// 
    /// Sample request:
    /// 
    ///     POST /Object2D
    ///     {
    ///         "prefabId": "samplePrefab",
    ///         "positionX": 10.0,
    ///         "positionY": 20.0,
    ///         "scaleX": 1.0,
    ///         "scaleY": 1.0,
    ///         "rotationZ": 0.0,
    ///         "sortingLayer": 0,
    ///         "environment2DID": "d290f1ee-6c54-4b01-90e6-d701748f0851",
    ///         "userID": "d290f1ee-6c54-4b01-90e6-d701748f0851"
    ///     }
    /// </remarks>
    [HttpPost(Name = "CreateObject")]
    public async Task<ActionResult> Add(Object2D Object)
    {
       Guid CurrentUser = Guid.Parse(_authenticationService.GetCurrentAuthenticatedUserId());
        Object.UserID = CurrentUser; // Set the UserID to the current authenticated user
        Object.Id = Guid.NewGuid();
        var createdObject = await _Object2DRepository.PostObjectAsync(Object);
        return Ok(createdObject);
    }

    /// <summary>
    /// Deletes an Object2D record by ID.
    /// </summary>
    /// <param name="ObjectId">The ID of the Object2D record to delete.</param>
    /// <returns>No content.</returns>
    /// <remarks>
    /// Route: DELETE /Object2D/{ObjectId}
    /// </remarks>
    [HttpDelete("{ObjectId}", Name = "DeleteObject")]
    public async Task<IActionResult> Delete(Guid ObjectId)
    {
        var existingObject = await _Object2DRepository.GetObjectAsync(ObjectId);

        if (existingObject == null)
            return NotFound();

        await _Object2DRepository.DeleteObjectAsync(ObjectId);

        return Ok();
    }

    /// <summary>
    /// Updates an existing Object2D record.
    /// </summary>
    /// <param name="ObjectId">The ID of the Object2D record to update.</param>
    /// <param name="NewObject">The updated Object2D record.</param>
    /// <returns>The updated Object2D record.</returns>
    /// <remarks>
    /// Route: PUT /Object2D/{ObjectId}
    /// 
    /// Sample request:
    /// 
    ///     PUT /Object2D/{ObjectId}
    ///     {
    ///         "id": "d290f1ee-6c54-4b01-90e6-d701748f0851",
    ///         "prefabId": "updatedPrefab",
    ///         "positionX": 15.0,
    ///         "positionY": 25.0,
    ///         "scaleX": 1.5,
    ///         "scaleY": 1.5,
    ///         "rotationZ": 45.0,
    ///         "sortingLayer": 1,
    ///         "environment2DID": "d290f1ee-6c54-4b01-90e6-d701748f0851",
    ///         "userID": "d290f1ee-6c54-4b01-90e6-d701748f0851"
    ///     }
    /// </remarks>
    [HttpPut("{ObjectId}", Name = "UpdateObject2D")]
    public async Task<ActionResult<Object2D>> Update(Guid ObjectId, Object2D NewObject)
    {
        var existingObject = await _Object2DRepository.GetObjectAsync(ObjectId);

        if (existingObject == null)
            return NotFound();

        await _Object2DRepository.UpdateObjectAsync(NewObject);

        return Ok(NewObject);
    }
}
