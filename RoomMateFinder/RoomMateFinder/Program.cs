using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Features.Profiles.DeleteProfile;
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Features.Profiles.GetProfileById;
using RoomMateFinder.Features.Profiles.CompleteOnboarding;

using RoomMateFinder.Features.Login.RegisterUser;
using RoomMateFinder.Features.Login.LoginUser;
using RoomMateFinder.Features.Matching.DislikeProfile;
using RoomMateFinder.Features.Matching.GetMatches;
using RoomMateFinder.Features.Matching.LikeProfile;
using RoomMateFinder.Features.Profiles.GetAllProfiles;
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using RoomMateFinder.Features.RoomListings.DeleteListing;
using RoomMateFinder.Features.RoomListings.GetAllListings;
using RoomMateFinder.Features.RoomListings.GetListingById;

using RoomMateFinder.Infrastructure.Persistence;
using RoomMateFinder.Middleware;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
         ?? "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=sirene99";

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseErrorHandling();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/profiles/{userId:guid}", async (Guid userId, CreateProfileRequest body, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(new CreateProfileCommand(userId, body), ct);
    return Results.Created($"/profiles/{id}", id);
});

app.MapGet("/profiles/{userId:guid}", async (Guid userId, IMediator mediator, CancellationToken ct) =>
{
    var profile = await mediator.Send(new GetProfileByIdQuery(userId), ct);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
});

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

app.MapLikeEndpoints();

app.MapCreateRoomListingEndpoint();
app.MapUpdateListingEndpoint();
app.MapDeleteListingEndpoint();
app.MapGetAllListingsEndpoint();
app.MapGetListingByIdEndpoint();
app.MapGetAllProfilesEndpoint();
app.MapDislikeEndpoint();
app.MapGetMatchesEndpoint();
app.Run();
