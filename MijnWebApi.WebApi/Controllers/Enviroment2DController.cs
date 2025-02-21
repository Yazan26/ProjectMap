using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes;

[ApiController]
[Route("[controller]")]

public class Enviroment2DController : ControllerBase
{

    [HttpPost(Name = "AddEnviroment")]

    public async Task<ActionResult> AddAsync(Enviroment2D enviroment)
    {
        enviroment.Id = Guid.NewGuid();

        var created = await _eniromentRepository.InsertAsync(enviroment);

        return CreatedAtRoute("GetEnviromentById", new { enviromentId = createdEnviroment.ID }, enviroment);
    }
}