namespace Vision_Media_Player.models;

public class PlaylistMedia
{
    public int PlaylistId { get; set; }
    public Playlist Playlist { get; set; } = null;
    
    public int MediaId { get; set; }
    public Media Media { get; set; } = null;
}


