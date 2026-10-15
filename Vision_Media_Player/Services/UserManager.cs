using Vision_Media_Player.Data;
using Vision_Media_Player.models;
using Vision_Media_Player.Services;

namespace Vision_Media_Player.Services;


public class UserManager
{
    private readonly AppDbContext _context;

    public UserManager(AppDbContext context)
    {
        _context = context;
    }
    
    // Get the user 
    public User? GetCurrentUser()
    {
        return _context.Users.FirstOrDefault();
    }
    // Create the user
    public User CreateUser(string username, string email)
    {
        var user = new User(username, email, DateTime.Now);
        _context.Users.Add(user);
        _context.SaveChanges();
        
        return user;
    }
    
    // Rename username
    public void UpdateUser(User user, string newUsername)
    {
        user.Name = newUsername;
        _context.SaveChanges();
    }

    // Change email
    public void ChangeEmail(User user, string newEmail)
    {
        user.Email = newEmail;
        _context.SaveChanges();
    }

}