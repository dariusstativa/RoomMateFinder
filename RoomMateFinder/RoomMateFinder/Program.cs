using System.Reflection;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RoomMateFinder.Features.Login;
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
         ?? "Host=localhost;Port=1326;Database=roommatefinder;Username=postgres;Password=tudor";

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        var keyString = builder.Configuration["Jwt:Key"];

        // aplicăm aceeași regulă de padding ca în JwtTokenGenerator
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
    });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RoomMateFinder API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                      "Enter your token like this: Bearer {token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthorization();


var app = builder.Build();
app.UseErrorHandling();
app.UseAuthentication();
app.UseAuthorization();
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

app.MapPost("/auth/register", async ([FromBody] RegisterRequest req, IMediator mediator) =>
{
    var id = await mediator.Send(new RegisterCommand(req));
    return Results.Created($"/users/{id}", id);
});

app.MapPost("/auth/login", async (LoginRequest req, IMediator mediator) =>
{
    var response = await mediator.Send(new LoginCommand(req));
    return Results.Ok(response);
});


app.MapCreateProfileEndpoint();
app.MapUpdateProfileEndpoint();
app.MapDeleteProfileEndpoint();
app.MapGetMyProfileEndpoint();
app.MapGetProfileByIdEndpoint();
app.MapGetAllProfilesEndpoint();

// matching
app.MapLikeEndpoints();
app.MapDislikeEndpoint();
app.MapGetMatchesEndpoint();

// room listings
app.MapCreateRoomListingEndpoint();
app.MapUpdateListingEndpoint();
app.MapDeleteListingEndpoint();
app.MapGetAllListingsEndpoint();
app.MapGetListingByIdEndpoint();
app.Run();
