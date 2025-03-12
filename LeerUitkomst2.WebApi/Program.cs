using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Runtime.InteropServices;
using LeerUitkomst2.WebApi.Repositories;
using LeerUitkomst2.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// "SqlConnectionString": "Server=(localdb)\MSSQLLocalDB;Database=avans2DGraphics1.3;Integrated Security=True;"
var sqlConnectionString = builder.Configuration["SqlConnectionString"];
var sqlConnectionStringNotFound = string.IsNullOrWhiteSpace(sqlConnectionString);
if (sqlConnectionStringNotFound)
    throw new InvalidProgramException("Configuration variable SqlConnectionString not found");

builder.Services.AddTransient<EnvironmentRepository, EnvironmentRepository>(o => new EnvironmentRepository(sqlConnectionString));
builder.Services.AddTransient<Object2DRepository, Object2DRepository>(o => new Object2DRepository(sqlConnectionString));

// Add services to the container.

builder.Services
    .AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        // add options
        options.Password.RequiredLength = 10;
    })
    .AddRoles<IdentityRole>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = sqlConnectionString;
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3030", builder =>
    {
        builder.WithOrigins("https://oldmartijntje.github.io")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
// And then in the pipeline:

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    
}


app.UseCors("AllowLocalhost3030");

string imageUrl = "https://i.imgur.com/JP3km0A.jpeg";

if (sqlConnectionStringNotFound)
{
    imageUrl = "https://i.imgur.com/I77WgZI.jpeg";
}
app.MapGet("/", () => Results.Content($@"<!DOCTYPE html>
<html lang = ""en"">
<head>
  <meta charset = ""UTF-8"">
  <meta name = ""viewport"" content = ""width=device-width, initial-scale=1.0"">
  <title> Random {(sqlConnectionStringNotFound ? "Dog" : "Cat")} </title>
  <style>
    html, body {{
margin: 0;
padding: 0;
height: 100 %;
    background - color: #000; /* optional: a dark background */
    }}
# catImage {{
width: 100 %;
height: 100 %;
object-fit: cover;
display: block;
    }}
  </style>
</head>
<body>
  <img id=""catImage"" src=""https://cataas.com/cat/says/click%20meh%20%3A3"" alt=""Random Cat"">
  
  <script>
    const catImage = document.getElementById('catImage');
catImage.addEventListener('click', () => {{
    catImage.src = 'https://cataas.com/cat?' + new Date().getTime();
}});
  </script>
</body>
</html>", "text/html"));

app.MapGet("/account/checkAccessToken", [Authorize] () => Results.Content("{\"authorized\": true}", "application/json"));


app.UseHttpsRedirection();

app.UseAuthorization();
app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers();

app.Run();
