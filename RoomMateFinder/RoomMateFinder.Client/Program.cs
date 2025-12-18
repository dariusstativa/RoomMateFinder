using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RoomMateFinder.Client;
using RoomMateFinder.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Auth service
builder.Services.AddScoped<AuthService>();

// The JWT handler
builder.Services.AddTransient<AuthorizationMessageHandler>();

// Principalul HttpClient folosit în aplicație
builder.Services.AddHttpClient("RoomMateFinderAPI", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5044");
    })
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// Simplify injection
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("RoomMateFinderAPI"));

await builder.Build().RunAsync();