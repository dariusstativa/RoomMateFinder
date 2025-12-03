<<<<<<< HEAD
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
using RoomMateFinder.Features.Matching.GetRecommendations;

using RoomMateFinder.Features.Profiles.GetAllProfiles;

using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using RoomMateFinder.Features.RoomListings.DeleteListing;
using RoomMateFinder.Features.RoomListings.GetAllListings;
using RoomMateFinder.Features.RoomListings.GetListingById;
=======
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using RoomMateFinder.Features.Login;
using RoomMateFinder.Features.Login.LoginUser;
using RoomMateFinder.Features.Login.RegisterUser;
using RoomMateFinder.Features.Matching.DislikeProfile;
using RoomMateFinder.Features.Matching.GetMatches;
using RoomMateFinder.Features.Matching.LikeProfile;
using RoomMateFinder.Features.Profiles.CompleteOnboarding;
using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Features.Profiles.DeleteProfile;
using RoomMateFinder.Features.Profiles.GetAllProfiles;
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Features.Profiles.GetProfileById;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Features.RoomListings.DeleteListing;
using RoomMateFinder.Features.RoomListings.GetAllListings;
using RoomMateFinder.Features.RoomListings.GetListingById;
using RoomMateFinder.Features.RoomListings.UpdateListing;
>>>>>>> DariusBranch

using RoomMateFinder.Infrastructure.Persistence;
using RoomMateFinder.Middleware;

var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
         ?? "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=3924";

// Services
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
=======
// ---------------------------------------------------------
// DATABASE
// ---------------------------------------------------------
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
         ?? "Host=localhost;Port=5432;Database=roommatefinder;Username=postgres;Password=sirene99";

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("RoomMateFinder_TestDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(cs));
}

// ---------------------------------------------------------
// MediatR + Validators
// ---------------------------------------------------------
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// ---------------------------------------------------------
// Swagger + JWT lock button
// ---------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RoomMateFinder API",
        Version = "v1"
    });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

// ---------------------------------------------------------
// JWT AUTH
// ---------------------------------------------------------
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (builder.Environment.IsEnvironment("Testing"))
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                ValidateLifetime = false,
                SignatureValidator = (token, _) => new JwtSecurityToken(token)
            };
        }
        else
        {
            var keyString = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyString) || keyString.Length < 32)
                keyString = (keyString ?? "").PadRight(32, '0');

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString))
            };
        }
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------
// JSON fixes
// ---------------------------------------------------------
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// ---------------------------------------------------------
// CORS
// ---------------------------------------------------------
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowBlazor", p =>
    {
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
    });
});
>>>>>>> DariusBranch

var app = builder.Build();

app.UseErrorHandling();
<<<<<<< HEAD

using (var scope = app.Services.CreateScope())
{
=======
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowBlazor");

// ---------------------------------------------------------
// DATABASE MIGRATIONS (not in tests!)
// ---------------------------------------------------------
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
>>>>>>> DariusBranch
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

<<<<<<< HEAD
=======
// ---------------------------------------------------------
// Swagger
// ---------------------------------------------------------
>>>>>>> DariusBranch
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

<<<<<<< HEAD

app.MapPost("/profiles/{userId:guid}", async (
    Guid userId,
    CreateProfileRequest body,
    IMediator mediator,
    CancellationToken ct) =>
{
    var id = await mediator.Send(new CreateProfileCommand(userId, body), ct);
    return Results.Created($"/profiles/{id}", id);
});

app.MapGet("/profiles/{userId:guid}", async (
    Guid userId,
    IMediator mediator,
    CancellationToken ct) =>
{
    var profile = await mediator.Send(new GetProfileByIdQuery(userId), ct);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
});

app.MapGet("/profiles/me", async (
    IMediator mediator,
    CancellationToken ct) =>
{
    // TODO: in the future, replace with auth user-id
    var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    var profile = await mediator.Send(new GetProfileQuery(userId), ct);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
});

app.MapPut("/profiles/{userId:guid}", async (
    Guid userId,
    UpdateProfileRequest body,
    IMediator mediator,
    CancellationToken ct) =>
{
    var ok = await mediator.Send(new UpdateProfileCommand(userId, body), ct);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/profiles/{userId:guid}", async (
    Guid userId,
    IMediator mediator,
    CancellationToken ct) =>
{
    var ok = await mediator.Send(new DeleteProfileCommand(userId), ct);
    return ok ? Results.NoContent() : Results.NotFound();
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


app.MapPost("/auth/register", async (
    [FromBody] RegisterRequest req,
    IMediator mediator) =>
{
    var id = await mediator.Send(new RegisterCommand(req));
    return Results.Created($"/users/{id}", id);
});

app.MapPost("/auth/login", async (
    LoginRequest req,
    IMediator mediator) =>
{
    Guid userId = await mediator.Send(new LoginCommand(req));
    return Results.Ok(userId);
});


app.MapLikeEndpoints();
app.MapDislikeEndpoint();
app.MapGetMatchesEndpoint();
app.MapRecommendationsEndpoint();


=======
// ---------------------------------------------------------
// AUTH ENDPOINTS
// ---------------------------------------------------------
app.MapPost("/auth/register", async (RegisterRequest req, IMediator mediator) =>
{
    var response = await mediator.Send(new RegisterCommand(req));
    return Results.Ok(response);
});

app.MapPost("/auth/login", async (LoginRequest req, IMediator mediator) =>
{
    var response = await mediator.Send(new LoginCommand(req));
    return Results.Ok(response);
});

// ---------------------------------------------------------
// PROFILE
// ---------------------------------------------------------
app.MapCreateProfileEndpoint();
app.MapUpdateProfileEndpoint();
app.MapDeleteProfileEndpoint();
app.MapGetMyProfileEndpoint();
app.MapGetProfileByIdEndpoint();
app.MapGetAllProfilesEndpoint();

// ---------------------------------------------------------
// MATCHING
// ---------------------------------------------------------
app.MapLikeEndpoints();
app.MapDislikeEndpoint();
app.MapGetMatchesEndpoint();

// ---------------------------------------------------------
// LISTINGS
// ---------------------------------------------------------
>>>>>>> DariusBranch
app.MapCreateRoomListingEndpoint();
app.MapUpdateListingEndpoint();
app.MapDeleteListingEndpoint();
app.MapGetAllListingsEndpoint();
app.MapGetListingByIdEndpoint();
<<<<<<< HEAD


app.MapGetAllProfilesEndpoint();

app.Run();
=======

app.Run();

namespace RoomMateFinder
{
    public partial class Program { }
}
>>>>>>> DariusBranch
