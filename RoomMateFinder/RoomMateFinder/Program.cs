using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Features.Profiles.GetMyProfile; 
using RoomMateFinder.Features.Profiles.UpdateProfile; 
using RoomMateFinder.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
         ?? "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=PAROLAVOASTRAAICI";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations automatically (optional, remove if you prefer manual control)
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


app.MapPost("/profiles/{userId:guid}", async (Guid userId, CreateProfileRequest body, IMediator mediator) =>
{
    var id = await mediator.Send(new CreateProfileCommand(userId, body));
    return Results.Created($"/profiles/{id}", id);
});


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

app.Run();
