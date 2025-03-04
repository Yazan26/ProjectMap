using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using MijnWebApi.WebApi.Classes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var connectionstring = builder.Configuration.GetValue<string>("connectionstring");
// Adding the HTTP Context accessor to be injected. This is needed by the AspNetIdentityUserRepository
// to resolve the current user.
builder.Configuration.AddUserSecrets<Program>(); // Add this line to read from user secrets
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
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
        options.ConnectionString = connectionstring;
    });


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MijnWebApi API", Version = "v1" });
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Connection string: {ConnectionString}", connectionstring);

static async Task SeedData(UserManager<IdentityUser> userManager)
{
    var user = await userManager.FindByEmailAsync("user@example.com");
    if (user != null)
    {
        await userManager.AddClaimAsync(user, new Claim("wizard", "true"));
        await userManager.AddClaimAsync(user, new Claim("wizardlevel", "8"));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await SeedData(userManager);
}

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
app.UseCors("AllowAllOrigins"); // added CORS
app.MapControllers();

app.MapGet(string.Empty, () => $"The API is up 🚀. Connection string found: {(connectionstring != null ? "✅" : "❌")}");

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

// map a register endpoint
app.MapPost("/account/register", async (UserManager<IdentityUser> userManager, [FromBody] RegisterModel model) =>
{
    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
    var result = await userManager.CreateAsync(user, model.Password);
    if (result.Succeeded)
    {
        return Results.Ok();
    }
    return Results.BadRequest(result.Errors);
});

//map a login endpoint
app.MapPost("/account/login", async (SignInManager<IdentityUser> signInManager, [FromBody] LoginModel model) =>
{
    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
    if (result.Succeeded)
    {
        return Results.Ok();
    }
    return Results.Unauthorized();
});

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