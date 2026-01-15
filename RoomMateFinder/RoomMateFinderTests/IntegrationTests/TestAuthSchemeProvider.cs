using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace RoomMateFinderTests.IntegrationTests;

public class TestAuthSchemeProvider : AuthenticationSchemeProvider
{
    public TestAuthSchemeProvider(IOptions<AuthenticationOptions> options)
        : base(options)
    {
    }
}