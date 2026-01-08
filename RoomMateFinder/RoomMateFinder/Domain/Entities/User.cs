

namespace RoomMateFinder.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
   
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "Student";
    public required string Salt { get; set; }

    public int Rating { get; set; } = 1200;

    public Profile? Profile { get; set; }
}
