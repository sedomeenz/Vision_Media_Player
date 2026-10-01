using Vision_Media_Player.models;

namespace Vision_Media_Player.Services;

public class MediaManager
{
    private readonly List<Media> _media = new();
    private readonly MediaFileManager _mediaFileManager;

    public MediaManager()
    {
        _mediaFileManager = new MediaFileManager();
    }

    // Adding new media through file path
    public List<string> AddMediaThroughFilePath(List<string> filepaths)
    {
        var results = new List<string>();

        foreach (var filepath in filepaths)
        {
            if (!_mediaFileManager.FileExists(filepath))
            {
                results.Add($"{filepath} Not Found ✘");
                continue;
            }

            results.Add($"{filepath} Found Successfully ✔");
        }

        return results;
    }
    
    
    /*
    public List<string> AddMediaThroughFolderPaths(string folderPath)
    {
        
    }
    */  

    // Removing list of chosen media
    public List<string> RemoveMedia(List<string> filenames)
    {
        var results = new List<string>();

        foreach (var filename in filenames)
        {
            var media = _media.FirstOrDefault(x =>
                Path.GetFileName(x.FilePath)
                    .Equals(filename, StringComparison.OrdinalIgnoreCase));

            if (media != null)
            {
                _media.Remove(media);
                results.Add($"{filename} Has Been Removed Successfully ✔");
            }
            else
            {
                results.Add($"{filename} Not Found ✘");
            }
        }

        return results;
    }

    // Displaying all media
    public List<Media> DisplayAllMedia()
    {
        return _media;
    }

    // Displaying all videos
    public List<Media> DisplayAllVideos()
    {
        return _media
            .Where(x => x.Type == MediaType.Video)
            .ToList();
    }

    // Displaying all audio
    public List<Media> DisplayAllAudio()
    {
        return _media
            .Where(x => x.Type == MediaType.Audio)
            .ToList();
    }

    // Searching media by title
    public List<Media> SearchMediaByTitle(string title)
    {
        var results = new List<Media>();

        foreach (var media in _media)
        {
            if (media.Name.Contains(title, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(media);
            }
        }

        return results;
    }
}