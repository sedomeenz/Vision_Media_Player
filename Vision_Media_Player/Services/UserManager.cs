namespace Vision_Media_Player.Services;

public class UserManager
{
    private readonly User _users;
    
    // Rename username
    public void RenameUsername(User user, string newUsername)
    {
        user.Name = newUsername;
    }
    
    // Change the Email
    public void ChangEmail(User user, string newEmail)
    {
        user.Email = newEmail;
    }
}