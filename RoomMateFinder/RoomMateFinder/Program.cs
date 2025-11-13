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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
using RoomMateFinder.Features.Profiles.CreateProfile;
<<<<<<< Updated upstream
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Features.RoomListings; 
=======
>>>>>>> Stashed changes
=======
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Features.Profiles.GetProfileById;
using RoomMateFinder.Features.Profiles.CompleteOnboarding;
>>>>>>> Stashed changes
using RoomMateFinder.Infrastructure.Persistence;
using System.Security.Claims;

using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Features.Profiles.UpdateProfile;

using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Features.RoomListings.GetListingById;
using RoomMateFinder.Features.RoomListings.GetAllListings;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using RoomMateFinder.Features.RoomListings.DeleteListing;
=======

using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Features.Profiles.DeleteProfile;
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Features.Profiles.GetProfileById;
using RoomMateFinder.Features.Profiles.CompleteOnboarding;

using RoomMateFinder.Features.Login.RegisterUser;
using RoomMateFinder.Features.Login.LoginUser;

using RoomMateFinder.Features.Matching.LikeProfile;
>>>>>>> Stashed changes

using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using RoomMateFinder.Features.RoomListings.DeleteListing;
using RoomMateFinder.Features.RoomListings.GetAllListings;
using RoomMateFinder.Features.RoomListings.GetListingById;

using RoomMateFinder.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
         ?? "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=PAROLA_TA_AICI";
=======
         ?? "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=admin123";
>>>>>>> Stashed changes
=======
         ?? "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=3924";
>>>>>>> Stashed changes

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

=======
         ?? "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=3924";

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
<<<<<<< Updated upstream
app.UseHttpsRedirection();
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======

>>>>>>> Stashed changes

>>>>>>> Stashed changes

var summaries = new[]
{
<<<<<<< Updated upstream
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
=======
    var id = await mediator.Send(new CreateProfileCommand(userId, body));
    return Results.Created($"/profiles/{id}", id);
});

>>>>>>> Stashed changes
app.MapGet("/profiles/{userId:guid}", async (Guid userId, IMediator mediator) =>
=======
app.MapPost("/profiles/{userId:guid}", async (Guid userId, CreateProfileRequest body, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(new CreateProfileCommand(userId, body), ct);
    return Results.Created($"/profiles/{id}", id);
});

app.MapPut("/profiles/{userId:guid}", async (Guid userId, UpdateProfileRequest body, IMediator mediator, CancellationToken ct) =>
>>>>>>> Stashed changes
{
    var ok = await mediator.Send(new UpdateProfileCommand(userId, body), ct);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapGet("/profiles/{userId:guid}", async (Guid userId, IMediator mediator, CancellationToken ct) =>
{
=======
app.MapPost("/profiles/{userId:guid}", async (Guid userId, CreateProfileRequest body, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(new CreateProfileCommand(userId, body), ct);
    return Results.Created($"/profiles/{id}", id);
});

app.MapGet("/profiles/{userId:guid}", async (Guid userId, IMediator mediator, CancellationToken ct) =>
{
>>>>>>> Stashed changes
    var profile = await mediator.Send(new GetProfileByIdQuery(userId), ct);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
});

<<<<<<< Updated upstream
<<<<<<< Updated upstream
app.MapPut("/profiles/{userId:guid}", async (Guid userId, UpdateProfileRequest body, IMediator mediator) =>
=======
app.MapGet("/profiles/me", async (IMediator mediator, CancellationToken ct) =>
>>>>>>> Stashed changes
{
    var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    var profile = await mediator.Send(new GetProfileByIdQuery(userId), ct);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
});

app.MapPost("/profiles/{userId:guid}/onboarding", async (
    Guid userId,
    CompleteOnboardingRequest body,
    IMediator mediator,
    CancellationToken ct) =>
{
    var ok = await mediator.Send(new CompleteOnboardingCommand(userId, body), ct);
    return ok ? Results.NoContent() : Results.NotFound();
});
>>>>>>> Stashed changes


app.MapCreateListingEndpoint();
app.MapGetListingByIdEndpoint();
app.MapGetAllListingsEndpoint();
app.MapUpdateListingEndpoint();
app.MapDeleteListingEndpoint();
=======
app.MapGet("/profiles/me", async (IMediator mediator, CancellationToken ct) =>
{
    var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    var profile = await mediator.Send(new GetProfileQuery(userId), ct);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
});

app.MapPut("/profiles/{userId:guid}", async (Guid userId, UpdateProfileRequest body, IMediator mediator, CancellationToken ct) =>
{
    var ok = await mediator.Send(new UpdateProfileCommand(userId, body), ct);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/profiles/{userId:guid}", async (Guid userId, IMediator mediator, CancellationToken ct) =>
{
    var ok = await mediator.Send(new DeleteProfileCommand(userId), ct);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapPost("/profiles/{userId:guid}/onboarding", async (Guid userId, CompleteOnboardingRequest body, IMediator mediator, CancellationToken ct) =>
{
    var ok = await mediator.Send(new CompleteOnboardingCommand(userId, body), ct);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapPost("/auth/register", async ([FromBody] RegisterRequest req, IMediator mediator) =>
{
    var id = await mediator.Send(new RegisterCommand(req));
    return Results.Created($"/users/{id}", id);
});

app.MapPost("/auth/login", async (LoginRequest req, IMediator mediator) =>
{
    Guid userId = await mediator.Send(new LoginCommand(req));
    return Results.Ok(userId);
});
>>>>>>> Stashed changes


app.MapCreateListingEndpoint();
app.MapUpdateListingEndpoint();
app.MapDeleteListingEndpoint();
app.MapGetAllListingsEndpoint();
app.MapGetListingByIdEndpoint();

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