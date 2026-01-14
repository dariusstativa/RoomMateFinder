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

// Login
using RoomMateFinder.Features.Login;
using RoomMateFinder.Features.Login.LoginUser;
using RoomMateFinder.Features.Login.RegisterUser;

// Matching
using RoomMateFinder.Features.Matching.DislikeProfile;
using RoomMateFinder.Features.Matching.GetMatches;
using RoomMateFinder.Features.Matching.LikeProfile;
using RoomMateFinder.Features.Matching.GetRecommendations;

// Profiles
using RoomMateFinder.Features.Profiles.CompleteOnboarding;
using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Features.Profiles.DeleteProfile;
using RoomMateFinder.Features.Profiles.GetAllProfiles;
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Features.Profiles.GetProfileById;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Features.Profiles.SearchProfiles;

// Room listings
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Features.RoomListings.DeleteListing;
using RoomMateFinder.Features.RoomListings.GetAllListings;
using RoomMateFinder.Features.RoomListings.GetListingById;
using RoomMateFinder.Features.RoomListings.UpdateListing;

// Reviews
using RoomMateFinder.Features.Reviews.AddReviewListing;
using RoomMateFinder.Features.Reviews.AddReviewProfile;
using RoomMateFinder.Features.Reviews.DeleteReview;
using RoomMateFinder.Features.Reviews.GetReviewListing;
using RoomMateFinder.Features.Reviews.GetReviwesProfile;

// Conversations + SignalR
using RoomMateFinder.Features.Conversations;
using RoomMateFinder.Features.Messages; // ✅ GetConversationMessagesEndpoint (MapGetConversationMessages)
using RoomMateFinder.Features.Conversations.Messaging;
using RoomMateFinder.Features.Conversations.Messaging.Conversation;
using RoomMateFinder.Features.Messaging.Conversation;
using RoomMateFinder.Features.RoomListings.GetMyListings; // ✅ dacă aici ai MapSendMessageEndpoint / alte endpoints

using RoomMateFinder.Infrastructure.Persistence;
using RoomMateFinder.Middleware;

var builder = WebApplication.CreateBuilder(args);

//
// DATABASE
//
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("RoomMateFinder_TestDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(connectionString));
}

//
// MediatR + FluentValidation
//
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

//
// Swagger
//
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
        Description = "JWT Bearer {token}",
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

//
// JWT AUTH
//
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

// IMPORTANT: nu remapa claim types automat
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"] ?? "";
        if (key.Length < 32)
            key = key.PadRight(32, '0');

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key))
        };

        // ✅ CRITICAL pentru SignalR WebSockets: token din query string (?access_token=)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/chat"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

//
// JSON
//
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

//
// CORS (Blazor + SignalR)
//
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowBlazor", p =>
    {
        p.WithOrigins("http://localhost:5218", "https://localhost:5218")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials();
    });
});

//
// Controllers + SignalR
//
builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

//
// Swagger
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//
// Middleware order (safe)
//
app.UseRouting();
app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();
app.UseErrorHandling();

app.MapControllers();

// ✅ SignalR Hub
app.MapHub<ChatHub>("/hubs/chat");

//
// Migrations
//
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

//
// Auth endpoints
//
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

//
// Reviews
//
app.MapAddReviewForListingEndpoint();
app.MapGetReviewsForListingEndpoint();
app.MapAddReviewForProfileEndpoint();
app.MapGetReviewsForProfileEndpoint();
app.MapDeleteReviewEndpoint();

//
// Profiles
//
app.MapCreateProfileEndpoint();
app.MapUpdateProfileEndpoint();
app.MapDeleteProfileEndpoint();
app.MapGetMyProfileEndpoint();
app.MapGetProfileByIdEndpoint();
app.MapGetAllProfilesEndpoint();
app.MapSearchProfilesEndpoint();

//
// Matching
//
app.MapLikeEndpoints();
app.MapDislikeEndpoint();
// ⚠️ recomand să îl scoți când e stabil, ca să nu ai două contracte diferite:
app.MapLegacyLikeEndpoint();

app.MapGetMatchesEndpoint();
app.MapRecommendationsEndpoint();

//
// Listings
//
app.MapCreateRoomListingEndpoint();
app.MapUpdateListingEndpoint();
app.MapDeleteListingEndpoint();
app.MapGetAllListingsEndpoint();
app.MapGetListingByIdEndpoint();
app.MapIsMatchEndpoint();

//
// Conversations + Messages
//
app.MapGetOrCreateConversation();

// ✅ lipsea: load messages by conversationId
app.MapGetConversationMessages();
app.MapGetMyListingsEndpoint();
app.MapGetOrCreateListingConversation();
app.MapGetConversations();

app.Run();

namespace RoomMateFinder
{
    public partial class Program { }
}
