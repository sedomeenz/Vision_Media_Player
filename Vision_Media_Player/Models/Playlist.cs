namespace Vision_Media_Player.models;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null;
    
    public DateTime CreatedAt { get; private set; }

    public List<PlaylistMedia> PlaylistMedias { get; private set; } = new();

}