using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class Object2DController : ControllerBase
{
    private static List<Object2D> objects = new List<Object2D>();

    [HttpGet]
    public ActionResult<IEnumerable<Object2D>> Get()
    {
        return objects;
    }

    [HttpGet("{name}")]
    public ActionResult<Object2D> Get(string name)
    {
        var obj = objects.FirstOrDefault(o => o.Name == name);
        if (obj == null)
        {
            return NotFound();
        }
        return obj;
    }

    [HttpPost]
    public ActionResult Add(Object2D obj)
    {
        objects.Add(obj);
        return CreatedAtAction(nameof(Get), new { name = obj.Name }, obj);
    }

    [HttpPut("{name}")]
    public ActionResult Update(string name, Object2D updatedObj)
    {
        var obj = objects.FirstOrDefault(o => o.Name == name);
        if (obj == null)
        {
            return NotFound();
        }
        obj.X = updatedObj.X;
        obj.Y = updatedObj.Y;
        return NoContent();
    }

    [HttpDelete("{name}")]
    public ActionResult Delete(string name)
    {
        var obj = objects.FirstOrDefault(o => o.Name == name);
        if (obj == null)
        {
            return NotFound();
        }
        objects.Remove(obj);
        return NoContent();
    }
}