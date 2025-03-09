using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


[ApiController]
[Route("[controller]")]
[Authorize]
public class Enviroment2DController : ControllerBase
{
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
        environments.Add(environment);
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