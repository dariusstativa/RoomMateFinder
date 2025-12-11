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
using RoomMateFinder.Features.Matching.GetRecommendations;

using RoomMateFinder.Features.Profiles.CompleteOnboarding;
using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Features.Profiles.DeleteProfile;
using RoomMateFinder.Features.Profiles.GetAllProfiles;
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Features.Profiles.GetProfileById;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Features.Reviews.AddReviewListing;
using RoomMateFinder.Features.Reviews.AddReviewProfile;
using RoomMateFinder.Features.Reviews.DeleteReview;
using RoomMateFinder.Features.Reviews.GetReviewListing;
using RoomMateFinder.Features.Reviews.GetReviwesProfile;
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Features.RoomListings.DeleteListing;
using RoomMateFinder.Features.RoomListings.GetAllListings;
using RoomMateFinder.Features.RoomListings.GetListingById;
using RoomMateFinder.Features.RoomListings.UpdateListing;

using RoomMateFinder.Infrastructure.Persistence;
using RoomMateFinder.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// DATABASE
// ---------------------------------------------------------
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
         ?? "Host=localhost;Port=1326;Database=roommatefinder;Username=postgres;Password=tudor";

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

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (builder.Environment.IsEnvironment("Testing"))
        {
            // Very relaxed validation in tests
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
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.UseErrorHandling();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowBlazor");

// ---------------------------------------------------------
// DATABASE MIGRATIONS (not in tests!)
// ---------------------------------------------------------
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// ---------------------------------------------------------
// Swagger
// ---------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
app.MapAddReviewForListingEndpoint();
app.MapGetReviewsForListingEndpoint();
app.MapAddReviewForProfileEndpoint();
app.MapGetReviewsForProfileEndpoint();
app.MapDeleteReviewEndpoint();

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
app.MapLegacyLikeEndpoint(); 
app.MapGetMatchesEndpoint();
app.MapRecommendationsEndpoint();

// ---------------------------------------------------------
// LISTINGS
// ---------------------------------------------------------
app.MapCreateRoomListingEndpoint();
app.MapUpdateListingEndpoint();
app.MapDeleteListingEndpoint();
app.MapGetAllListingsEndpoint();
app.MapGetListingByIdEndpoint();

app.Run();

namespace RoomMateFinder
{
    public partial class Program { }
}
