namespace Vision_Media_Player.models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime JoinedIn {get; private  set;}

    public List<Playlist> Playlists { get; set; } = new();
    
    public User(string name, string email, DateTime joinedIn)
    {
        Name = name;
        Email = email;
        JoinedIn = joinedIn;
    }
}