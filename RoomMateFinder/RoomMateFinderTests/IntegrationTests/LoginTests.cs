using System.Net;
using System.Net.Http.Json;
using Xunit;

public class LoginTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Should_Return_Token_For_Valid_Credentials()
    {
       
        var email = "loginuser@example.com";
        var password = "Password123!";

        var registerPayload = new { email, password };
        var registerResponse = await _client.PostAsJsonAsync("/auth/register", registerPayload);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        
        var loginPayload = new { email, password };
        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginPayload);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body!.UserId);
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
    }

    [Fact]
    public async Task Login_Should_Return_500_For_Wrong_Password()
    {
       
        var email = "wrongpass@example.com";
        var password = "Password123!";

        
        await _client.PostAsJsonAsync("/auth/register", new { email, password });

        
        var badResponse = await _client.PostAsJsonAsync("/auth/login",
            new { email, password = "WrongPass123!" });

       
        Assert.True(
            badResponse.StatusCode == HttpStatusCode.InternalServerError ||
            badResponse.StatusCode == HttpStatusCode.BadRequest ||
            badResponse.StatusCode == HttpStatusCode.Unauthorized
        );
    }

    private record LoginResponseDto(Guid UserId, string Token);
}
