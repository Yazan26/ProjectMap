using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes;

namespace MijnWebApi.WebApi.Controllers;

[ApiController]
[Route("weather")]
public class WeatherForecastController : ControllerBase
  
{
    private static List<WeatherForecast> forecasts = [];

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

   
    
    [HttpGet(Name = "GetWeatherForecast")]
    public ActionResult<IEnumerable<WeatherForecast>> Get()
    {
        return forecasts;
    }

    
    
    
    [HttpGet("{date:datetime}", Name = "GetWEatherForecastsByDate")]

    public ActionResult<IEnumerable<WeatherForecast>> Get(DateOnly date)
    {
        return forecasts.Where(x => x.Date == date).ToArray();
    }

    [HttpPost(Name = "AddWeatherForecast")]
    public ActionResult Add(WeatherForecast forecast)
    {
        forecasts.Add(forecast);
        return Created();
    }

}
