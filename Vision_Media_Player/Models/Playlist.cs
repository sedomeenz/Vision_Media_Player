namespace Vision_Media_Player.models;

public class Playlist
{
    public int Id { get; set; }
    
    public string Name { get; set; } =  string.Empty;
    
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public List<PlaylistMedia> PlaylistMedias { get; private set; } = new();

}