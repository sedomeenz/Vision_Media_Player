namespace Vision_Media_Player.models;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Media> Items { get; } = new();
    public int ItemsCount => Items.Count;
}