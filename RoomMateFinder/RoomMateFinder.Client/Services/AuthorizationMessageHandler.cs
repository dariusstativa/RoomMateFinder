using System.Net.Http.Headers;

namespace RoomMateFinder.Client.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public AuthorizationMessageHandler(AuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var token = _authService.Token;
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"✅ JWT Token added to request: {request.RequestUri}");
            Console.WriteLine($"Token (first 20 chars): {token.Substring(0, Math.Min(20, token.Length))}...");
        }
        else
        {
            Console.WriteLine($"⚠️ No JWT token available for request: {request.RequestUri}");
        }

        var response = await base.SendAsync(request, cancellationToken);
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine($"❌ 401 Unauthorized response from: {request.RequestUri}");
        }

        return response;
    }
}
