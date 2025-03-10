using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User, Admin")]
public class Enviroment2DController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<Enviroment2DController> _logger;
    public Enviroment2DController(IAuthenticationService authService, ILogger<Enviroment2DController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    private static List<Enviroment2D> environments = new List<Enviroment2D>();

    [HttpGet]
    public ActionResult<IEnumerable<Enviroment2D>> Get()
    {
        return environments;
    }

    [HttpGet("{name}")]
    public ActionResult<Enviroment2D> Get(string name)
    {
        var environment = environments.FirstOrDefault(e => e.Name == name);
        if (environment == null)
        {
            return NotFound();
        }
        return environment;
    }

    [HttpPost]
    public ActionResult Add(Enviroment2D environment)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        _logger.LogInformation("Authenticated User ID: {UserId}", userId);

        var userClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
        _logger.LogInformation(" Received Claims: {Claims}", string.Join(", ", userClaims));

        if (!User.IsInRole("User") && !User.IsInRole("Admin"))
        {
            _logger.LogWarning(" User {UserId} does NOT have the required role!", userId);
            return Unauthorized(new { message = "You do not have permission to create worlds." });
        }

        environments.Add(environment);
        _logger.LogInformation(" World '{WorldName}' created successfully by User ID: {UserId}", environment.Name, userId);

        return CreatedAtAction(nameof(Get), new { name = environment.Name }, environment);
    }


[HttpPut("{name}")]
    public ActionResult Update(string name, Enviroment2D updatedEnvironment)
    {
        var environment = environments.FirstOrDefault(e => e.Name == name);
        if (environment == null)
        {
            return NotFound();
        }
        environment.MaxLength = updatedEnvironment.MaxLength;
        environment.MaxHeight = updatedEnvironment.MaxHeight;
        return NoContent();
    }

    [HttpDelete("{name}")]
    public ActionResult Delete(string name)
    {
        var environment = environments.FirstOrDefault(e => e.Name == name);
        if (environment == null)
        {
            return NotFound();
        }
        environments.Remove(environment);
        return NoContent();
    }
}