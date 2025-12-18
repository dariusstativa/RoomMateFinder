using Microsoft.JSInterop;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace RoomMateFinder.Client.Services;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigation;

    private string? _token;
    private Guid? _userId;

    public AuthService(IJSRuntime jsRuntime, NavigationManager navigation)
    {
        _jsRuntime = jsRuntime;
        _navigation = navigation;
    }

    /* =====================
       PUBLIC STATE
    ===================== */
    public string? Token => _token;
    public Guid? UserId => _userId;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_token) && _userId.HasValue;

    /* =====================
       AUTH STATE EVENT
    ===================== */
    public event Action? OnAuthStateChanged;

    private void NotifyAuthStateChanged()
    {
        OnAuthStateChanged?.Invoke();
    }

    /* =====================
       INITIALIZATION
    ===================== */
    // Called once at app startup (Program.cs)
    public async Task InitializeAsync()
    {
        try
        {
            _token = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem", "authToken");

            var userIdStr = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem", "userId");

            if (Guid.TryParse(userIdStr, out var userId))
            {
                _userId = userId;
            }

            Console.WriteLine(
                $"🔐 AuthService initialized - Token exists: {(!string.IsNullOrEmpty(_token))}, UserId: {_userId}");

            NotifyAuthStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ AuthService initialization failed: {ex.Message}");
        }
    }

    /* =====================
       LOGIN
    ===================== */
    public async Task LoginAsync(Guid userId, string token)
    {
        _userId = userId;
        _token = token;

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem", "authToken", token);

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem", "userId", userId.ToString());

        Console.WriteLine(
            $"✅ User logged in - UserId: {userId}, Token length: {token.Length}");

        NotifyAuthStateChanged();
    }

    /* =====================
       LOGOUT
    ===================== */
    public async Task LogoutAsync()
    {
        _userId = null;
        _token = null;

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem", "authToken");

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem", "userId");

        Console.WriteLine("🚪 User logged out");

        NotifyAuthStateChanged();

        // full reload – avoids stale state
        _navigation.NavigateTo("/login", forceLoad: true);
    }

    /* =====================
       HTTP HELPERS
    ===================== */
    public void AddAuthorizationHeader(HttpClient httpClient)
    {
        if (!string.IsNullOrEmpty(_token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            Console.WriteLine("🔑 Authorization header added");
        }
        else
        {
            Console.WriteLine("⚠️ No token available");
        }
    }

    public HttpRequestMessage CreateAuthenticatedRequest(
        HttpMethod method, string uri)
    {
        var request = new HttpRequestMessage(method, uri);

        if (!string.IsNullOrEmpty(_token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            Console.WriteLine($"📨 Authenticated request created: {method} {uri}");
        }
        else
        {
            Console.WriteLine($"⚠️ Request without token: {method} {uri}");
        }

        return request;
    }
}
