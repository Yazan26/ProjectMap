using System.ComponentModel.DataAnnotations;

namespace MijnWebApi.WebApi.Classes;

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    [Range(-75, 75)]

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    [Required]
    public string? Summary { get; set; }
}
