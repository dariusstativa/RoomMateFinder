using System.Net;
using System.Net.Http.Json;
using Xunit;

public class CreateAndGetMyProfileTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateAndGetMyProfileTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Full_Profile_Flow_Should_Work()
    {
        
        var email = "testuser@example.com";
        var password = "Password123!";

        var registerResp = await _client.PostAsJsonAsync("/auth/register", new
        {
            Email = email,
            Password = password
        });

        Assert.Equal(HttpStatusCode.OK, registerResp.StatusCode);

       
        var loginResp = await _client.PostAsJsonAsync("/auth/login", new
        {
            Email = email,
            Password = password
        });

        Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

        var loginJson = await loginResp.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(loginJson);
        Assert.False(string.IsNullOrWhiteSpace(loginJson!.Token));

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginJson.Token);

      
        var createBody = new
        {
            FullName = "Test User",
            Age = 21,
            Gender = "M",
            University = "UAIC",
            Bio = "Some bio",
            SleepSchedule = "Night owl",
            Cleanliness = "Clean",
           NoiseTolerance = "Low",
            SmokingPreference = "Non-smoker",
            PetPreference = "No pets",
            StudyHabits = "Hard-working"
        };

        var createResp = await _client.PostAsJsonAsync("/profiles", createBody);

        if (createResp.StatusCode != HttpStatusCode.Created)
        {
            var raw = await createResp.Content.ReadAsStringAsync();
            throw new Xunit.Sdk.XunitException(
                $"Expected 201 Created, got {(int)createResp.StatusCode} {createResp.StatusCode}. Body: {raw}");
        }

        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

        
        var meResp = await _client.GetAsync("/profiles/me");
        Assert.Equal(HttpStatusCode.OK, meResp.StatusCode);

        var me = await meResp.Content.ReadFromJsonAsync<ProfileDto>();
        Assert.NotNull(me);

        Assert.Equal("Test User", me!.FullName);
        Assert.Equal(21, me.Age);
        Assert.Equal("UAIC", me.University);
    }

    private sealed class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = "";
    }

    private sealed class ProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string University { get; set; }
        public string Bio { get; set; }
        public string SleepSchedule { get; set; }
        public string Cleanliness { get; set; }
        public string NoiseTolerance { get; set; }
        public string SmokingPreference { get; set; }
        public string PetPreference { get; set; }
        public string StudyHabits { get; set; }
    }
}
