using MuseSpace.API.Middleware;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Services;
using MuseSpace.Core.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IService<SampleDto>, SampleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
