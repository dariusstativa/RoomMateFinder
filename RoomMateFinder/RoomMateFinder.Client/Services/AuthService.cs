using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace RoomMateFinder.Client.Services;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;
    private string? _token;
    private Guid? _userId;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? Token => _token;
    public Guid? UserId => _userId;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_token) && _userId.HasValue;

    public async Task InitializeAsync()
    {
        try
        {
            _token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            var userIdStr = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "userId");
            if (Guid.TryParse(userIdStr, out var userId))
            {
                _userId = userId;
            }
            
            Console.WriteLine($"🔐 AuthService initialized - Token exists: {!string.IsNullOrEmpty(_token)}, UserId: {_userId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ AuthService initialization failed: {ex.Message}");
        }
    }

    public async Task LoginAsync(Guid userId, string token)
    {
        _userId = userId;
        _token = token;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", userId.ToString());
        
        Console.WriteLine($"✅ User logged in - UserId: {userId}, Token length: {token.Length}");
    }

    public async Task LogoutAsync()
    {
        _userId = null;
        _token = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
        
        Console.WriteLine("🚪 User logged out");
    }

    // Helper method to add Authorization header to HttpClient
    public void AddAuthorizationHeader(HttpClient httpClient)
    {
        if (!string.IsNullOrEmpty(_token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            Console.WriteLine($"✅ Authorization header added to HttpClient");
        }
        else
        {
            Console.WriteLine($"⚠️ No token available to add to HttpClient");
        }
    }

    // Helper method to create authenticated request message
    public HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, string requestUri)
    {
        var request = new HttpRequestMessage(method, requestUri);
        if (!string.IsNullOrEmpty(_token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            Console.WriteLine($"✅ Token added to {method} {requestUri}");
        }
        else
        {
            Console.WriteLine($"⚠️ No token available for {method} {requestUri}");
        }
        return request;
    }
}
