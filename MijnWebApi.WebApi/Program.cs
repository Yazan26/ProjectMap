using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using MijnWebApi.WebApi.Classes.Interfaces;
using MijnWebApi.WebApi.Classes.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load User Secrets (in Development)
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();

var sqlConnectionString = builder.Configuration.GetValue<string>("connectionstring");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

// Adding the HTTP Context accessor to be injected. This is needed by the AspNetIdentityUserRepository
// to resolve the current user.
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

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
        options.ConnectionString = sqlConnectionString;
    });

// ✅ CORS
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

// ✅ Swagger beveiliging voor Bearer tokens
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MijnWebApi API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Voer een geldig JWT-token in (zonder 'Bearer ' voor het token)"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// ✅ Register direct DB connection
builder.Services.AddScoped<IDbConnection>(sp =>
{
    logger.LogInformation("🔗 Attempting to create a database connection...");
    return new SqlConnection(sqlConnectionString);
});

// ✅ Register IdentityService and Repositories
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IEnvironment2DRepository, Environment2DRepository>();
builder.Services.AddScoped<IObject2DRepository, Object2DRepository>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();

// Keep default Identity endpoints mapped under /auth.
app.MapGroup("/auth")
    .MapIdentityApi<IdentityUser>();

// Custom logout endpoint.
app.MapPost("/auth/logout",
    async (SignInManager<IdentityUser> signInManager, [FromBody] object empty) =>
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    })
.RequireAuthorization();

async Task EnsureRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "User", "Admin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

using (var scope = app.Services.CreateScope()) // ✅ Run Role Setup Before API Starts
{
    var services = scope.ServiceProvider;
    await EnsureRolesAsync(services);
}

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
app.UseAuthentication(); // ✅ Authenticatie wordt nu gebruikt
app.UseAuthorization();
app.UseCors("AllowAllOrigins"); // ✅ CORS 
app.MapControllers();

// Map Identity API endpoints under the 'account' prefix
app.MapGroup("/account")
    .MapIdentityApi<IdentityUser>();

// Map controllers and require authorization by default
app.MapControllers()
    .RequireAuthorization();

app.MapGet("/", () => $"The API is up 🚀. Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")}");

app.Use(async (context, next) =>
{
    Console.WriteLine($"📌 Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
});

app.Run();