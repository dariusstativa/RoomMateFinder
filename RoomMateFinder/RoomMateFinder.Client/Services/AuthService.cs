namespace RoomMateFinder.Client.Services;

public class AuthService
{
    private Guid? _userId;

    public Guid? UserId
    {
        get => _userId;
        set => _userId = value;
    }

    public bool IsAuthenticated => _userId.HasValue;

    public void Login(Guid userId)
    {
        _userId = userId;
    }

    public void Logout()
    {
        _userId = null;
    }
}

