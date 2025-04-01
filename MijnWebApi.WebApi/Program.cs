using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Avans.Identity.Dapper;
using Dapper;
using System.Data;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using MijnWebApi.WebApi.Classes.Services;
using MijnWebApi.WebApi.Classes.Interfaces;
using MijnWebApi.WebApi.Classes.Models;
using MijnWebApi.WebApi.Classes.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//connection string
var sqlConnectionString = builder.Configuration.GetValue<string>("connectionstring");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);
// Configure logging
var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
    loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
});
var logger = loggerFactory.CreateLogger<Program>();
// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();
builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(sqlConnectionString));
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEnvironment2DRepository>(provider =>
    new Environment2DRepository(sqlConnectionString));

builder.Services.AddScoped<IObject2DRepository>(provider =>
    new Object2DRepository(sqlConnectionString));


// Configure authorization
builder.Services.AddAuthorization();

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

// Configure Identity with Dapper stores
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 10;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
    })
    .AddRoles<IdentityRole>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = builder.Configuration
        .GetValue<string>("connectionstring"); // Add connection string in user secrets for localhost
    });

var app = builder.Build();

// Endpoint for logging out
app.MapPost("/account/logout",
    async (SignInManager<IdentityUser> SignInManager,
    [FromBody] object empty) =>
    {
        if (empty != null)
        {
            await SignInManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    })
    .RequireAuthorization();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map identity and controller endpoints
app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers().RequireAuthorization();

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Configure HTTPS redirection
app.UseHttpsRedirection();

// Health check endpoint
app.MapGet("/", () => $"Yazans project WebAPI is up. Connection string found: {(sqlConnectionStringFound ? "Yes" : "No")}");



// Configure authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
