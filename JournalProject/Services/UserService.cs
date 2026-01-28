namespace JournalProject.Services;

public class UserService
{
    private string? _currentUser;
    
    public bool IsLoggedIn { get; set; } = false;
    
    public User? CurrentUser { get; set; }
    
    public void Login(string username)
    {
        _currentUser = username;
        IsLoggedIn = true;
        CurrentUser = new User { Name = username };
    }
    
    public void Logout()
    {
        _currentUser = null;
        IsLoggedIn = false;
        CurrentUser = null;
    }
}

public class User
{
    public string Name { get; set; } = string.Empty;
}
