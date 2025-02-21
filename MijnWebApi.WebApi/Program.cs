var builder = WebApplication.CreateBuilder(args);

var SQLConnectionString = builder.Configuration["SqlConnectionString"];


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// SQLConnectionString
var sqlConnectionString = builder.COnfiguration.GetValue<string>("SqlConnectionString");
var sqlCOnnectionStringFOund = !string.IsNullOrWhiteSpace(sqlConnectionString);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
