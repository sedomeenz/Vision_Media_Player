using System.Reflection.Metadata.Ecma335;

namespace Vision_Media_Player.Services;

public class MediaFileManager
{
    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
        var text = filePath;
    }
}