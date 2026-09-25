namespace Vision_Media_Player;

public class Playlist: Media
{
    public List<Media> Items { get; } = new();
    public int ItemsCount => Items.Count;
}