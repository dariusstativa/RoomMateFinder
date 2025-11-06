<<<<<<< Updated upstream
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
=======
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Features.RoomListings; 
using RoomMateFinder.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
         ?? "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=PAROLA_TA_AICI";

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

>>>>>>> Stashed changes
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

<<<<<<< Updated upstream
app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");
=======
app.MapGet("/profiles/{userId:guid}", async (Guid userId, IMediator mediator) =>
{
    var profile = await mediator.Send(new GetProfileQuery(userId));
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
});

app.MapPut("/profiles/{userId:guid}", async (Guid userId, UpdateProfileRequest body, IMediator mediator) =>
{
    var ok = await mediator.Send(new UpdateProfileCommand(userId, body));
    return ok ? Results.NoContent() : Results.NotFound();
});
>>>>>>> Stashed changes


app.MapCreateListingEndpoint();
app.MapGetListingByIdEndpoint();
app.MapGetAllListingsEndpoint();
app.MapUpdateListingEndpoint();
app.MapDeleteListingEndpoint();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}