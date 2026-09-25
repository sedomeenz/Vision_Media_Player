namespace Vision_Media_Player;

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime JoinedIn {get; private  set;}
    
    public User(string name, string email, DateTime joinedIn)
    {
        Name = name;
    }
    
    public List<Playlist> Playlists { get; set; } = new List<Playlist>();

    public string Username
    {
        get
        {
            return '@' + Name;
        }
    }
}