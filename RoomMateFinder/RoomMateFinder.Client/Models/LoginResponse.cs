namespace RoomMateFinder.Client.Models;

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}
