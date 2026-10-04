using Vision_Media_Player.Data;
using Vision_Media_Player.models;
using Vision_Media_Player.Services;

namespace Vision_Media_Player.Services;


public class UserManager
{
    private User _userManger;
    private readonly AppDbContext _context;

    public UserManager(User user, AppDbContext context)
    {
        _userManger = user;
        _context = context;
    }
    
    // Rename username
    public void RenameUsername(User user, string newUsername)
    {
        
    }
    
    // Change the Email
    public void ChangEmail(User user, string newEmail)
    {
        user.Email = newEmail;
    }
}