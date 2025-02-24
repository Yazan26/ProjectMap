var builder = WebApplication.CreateBuilder(args);

var SQLConnectionString = builder.Configuration["SqlConnectionString2"];
// aaa

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// SQLConnectionString
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString2");
bool sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);


app.MapGet("/", () => $"The API is up 🚀. Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")}");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
