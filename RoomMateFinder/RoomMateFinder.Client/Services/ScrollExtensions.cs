using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace RoomMateFinder.Client.Services;
public static class ScrollExtensions
{
    public static async Task ScrollToEndAsync(this ElementReference element, IJSRuntime js)
    {
        await js.InvokeVoidAsync("scrollHelpers.scrollToEnd", element);
    }
}