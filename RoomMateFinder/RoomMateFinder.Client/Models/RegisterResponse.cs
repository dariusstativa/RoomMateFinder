namespace RoomMateFinder.Client.Models;

public class RegisterResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}
