namespace Vision_Media_Player.models;

public class Media
{
    public int Id { get; set; }
    public string Name { get; set; }
    public MediaType Type { get; set; }
    public string FilePath { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime CreatedAt { get; private set; }
    
}