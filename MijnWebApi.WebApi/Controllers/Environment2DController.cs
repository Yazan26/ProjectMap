using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MijnWebApi.WebApi.Classes.Models;



namespace MijnWebApi.WebApi.Controllers
{
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
                return NotFound("geen wereld gevonden met dit ID, maak snel nieuwe!");
            }
            return Ok(world);
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
            if (Worlds == null)
            {
                return NotFound("geen werelden gevonden, maak snel nieuwe!");
            }
            return Ok(Worlds);
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
                return NotFound("wereld niet gevonden");

            await _environment2DRepository.UpdateWorldAsync(NewWorld);

            return Ok(NewWorld);
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
                    return BadRequest("name already exists");
                }
            }
            if (worlds.Count() >= 5)
            {
                return BadRequest("je hebt meer dan 5 werelden! verwijder 1");
            }
            else if (NameExists == true)
            {
                return BadRequest("name already exists");
            }
            else
            {
                World.Id = Guid.NewGuid();
                World.OwnerUserID = CurrentUser;
                var CreatedWorld = await _environment2DRepository.PostWorldAsync(World);
                return Ok("world created successfully");
            }
        }

        /// <summary>
        /// Deletes an Environment2D record by ID.
        /// </summary>
        /// <param name="Environment2DId">The ID of the Environment2D record to delete.</param>
        /// <returns>Action result.</returns>
        /// <remarks>
        /// Route: DELETE /Environment2D/{Environment2DId}
        /// </remarks>
        [HttpDelete("{Environment2DId}", Name = "DeleteWorld")]
        public async Task<ActionResult> Delete(Guid Environment2DId)
        {
            var ExistingWorld = await _environment2DRepository.GetWorldAsync(Environment2DId);
            if (ExistingWorld == null)
            {
                return NotFound("invalid world id!");
            }
            else
            {
                await _environment2DRepository.DeleteWorldAsync(Environment2DId);
                return Ok("world deleted");
            }
        }
    }
}
