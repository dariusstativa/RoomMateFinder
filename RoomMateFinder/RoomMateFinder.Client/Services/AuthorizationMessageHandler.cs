using System.Net.Http.Headers;
using Microsoft.JSInterop;
namespace RoomMateFinder.Client.Services;
public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _js;

    public AuthorizationMessageHandler(IJSRuntime js)
    {
        _js = js;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // IMPORTANT: citim tokenul exact înainte de request
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine("Handler trimite JWT.");
        }
        else
        {
            Console.WriteLine("❌ Handler: NU exista token în localStorage");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}