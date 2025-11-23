using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class TestAuthSchemeProvider : AuthenticationSchemeProvider
{
    public TestAuthSchemeProvider(IOptions<AuthenticationOptions> options)
        : base(options)
    {
    }

    public override Task<AuthenticationScheme?> GetDefaultAuthenticateSchemeAsync()
    {
        var scheme = new AuthenticationScheme(
            "Test",          
            "Test Scheme",    
            typeof(TestAuthHandler));  

        return Task.FromResult<AuthenticationScheme?>(scheme);
    }

    public override Task<AuthenticationScheme?> GetDefaultChallengeSchemeAsync()
        => GetDefaultAuthenticateSchemeAsync();
}