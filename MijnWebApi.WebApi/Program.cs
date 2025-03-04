using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Adding the HTTP Context accessor to be injected. This is needed by the AspNetIdentityUserRepository
// to resolve the current user.
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Level8WizardOnly", policy =>
    {
        policy.RequireClaim("wizard");
        policy.RequireClaim("wizardlevel", 8.ToString());
    });
});

builder.Services
    .AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
    })
    .AddRoles<IdentityRole>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("DapperIdentity");
    });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MijnWebApi API", Version = "v1" });
});


static async Task SeedData(UserManager<IdentityUser> userManager)
{
    var user = await userManager.FindByEmailAsync("user@example.com");
    if (user != null)
    {
        await userManager.AddClaimAsync(user, new Claim("wizard", "true"));
        await userManager.AddClaimAsync(user, new Claim("wizardlevel", "8"));
    }
}



var app = builder.Build();
await SeedData(app.Services.GetRequiredService<UserManager<IdentityUser>>());

// SQLConnectionString
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
bool sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MijnWebApi API V1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet(string.Empty, () => $"The API is up 🚀. Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")}");

// Map Identity API endpoints under the 'account' prefix
app.MapGroup("/account")
    .MapIdentityApi<IdentityUser>();

// Map a logout endpoint
app.MapPost("/account/logout",
    async (SignInManager<IdentityUser> signInManager,
    [FromBody] object empty) => {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    })
    .RequireAuthorization();

// Map controllers and require authorization by default
app.MapControllers()
    .RequireAuthorization();

app.Run();

[Authorize]
public class WizardController : ControllerBase
{
    private readonly ILogger<WizardController> _logger;
    public WizardController(ILogger<WizardController> logger)
    {
        _logger = logger;
    }
    [HttpGet]
    public string Get()
    {
        _logger.LogInformation("Get method called");
        return "Hello from the Wizard!";
    }
}