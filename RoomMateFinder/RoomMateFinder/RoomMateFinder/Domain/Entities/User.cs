namespace RoomMateFinder.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "Student";  
    public string Salt { get; set; }

    
    public Profile? Profile { get; set; }
}
