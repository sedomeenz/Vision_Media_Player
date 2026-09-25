namespace Vision_Media_Player;

public class Media
{
    public string Title { get; set; }
    public string FilePath { get; set; }
    public string Owner { get; }
    public TimeSpan Duration { get; set; }
    public DateTime CreatedAt { get; private set; }
}