using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class CustomWebApplicationFactory
    : WebApplicationFactory<RoomMateFinder.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "THIS_IS_A_TEST_KEY_123456789012345",
                ["Jwt:Issuer"] = "RoomMateFinderTests",
                ["Jwt:Audience"] = "RoomMateFinderTests"
            };

            configBuilder.AddInMemoryCollection(dict);
        });

        builder.ConfigureServices(services =>
        {
          
            services.RemoveAll<IAuthenticationSchemeProvider>();

            
            services.AddSingleton<IAuthenticationSchemeProvider, TestAuthSchemeProvider>();

           
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        });
    }
}