using ProjectMap.WebApi.Repositories;


var builder = WebApplication.CreateBuilder(args);

// "SqlConnectionString": "Server=(localdb)\MSSQLLocalDB;Database=avans2DGraphics1.3;Integrated Security=True;"
var sqlConnectionString = builder.Configuration["SqlConnectionString"];
var sqlConnectionStringNotFound = string.IsNullOrWhiteSpace(sqlConnectionString);
if (sqlConnectionStringNotFound)
    throw new InvalidProgramException("Configuration variable SqlConnectionString not found");

builder.Services.AddTransient<EnvironmentRepository, EnvironmentRepository>(o => new EnvironmentRepository(sqlConnectionString));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


string imageUrl = "https://i.imgur.com/JP3km0A.jpeg";

if (sqlConnectionStringNotFound)
{
    imageUrl = "https://i.imgur.com/I77WgZI.jpeg";
}
app.MapGet("/", () => Results.Content(@$" <!DOCTYPE html>
<html>
    <head>
        <title>Hehe haw = {sqlConnectionStringNotFound}</title>
    </head>
    <body>
        <img src=""{imageUrl}"">
    </body>
</html> ", "text/html"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
