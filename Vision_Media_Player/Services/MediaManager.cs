using Microsoft.EntityFrameworkCore;
using Vision_Media_Player.models;
using Vision_Media_Player.Data;

namespace Vision_Media_Player.Services;

public class MediaManager
{
    private readonly AppDbContext _context;
    private readonly MediaStorageManager _storageManager;

    public MediaManager(AppDbContext context, MediaStorageManager storageManager)
    {
        _context = context;
        _storageManager = storageManager;
    }

    // Adding new media through file path
    public List<string> AddMediaThroughFilePath(List<string> filePath)
    {
        var results = new List<string>();

        foreach (var filepath in filePath)
        {

            if (!File.Exists(filepath))
            {
                results.Add($"{filepath} Not Found ✘");
                continue;
            }

            try
            {
                MediaType mediaType = GetMediaType(filepath);

                string copiedFilePath = _storageManager.CopyMediaFile(filepath, mediaType);

                string relativePath = _storageManager.GetRelativePath(copiedFilePath);

                var media = new Media
                {
                    Name = Path.GetFileNameWithoutExtension(filepath),
                    Type = mediaType,
                    FilePath = relativePath,
                    Duration = TimeSpan.Zero,
                    CreatedAt =  DateTime.Now
                };

                _context.Medias.Add(media);

                results.Add($"{Path.GetFileName(filepath)} Added Successfully ✔");
            }
            catch (NotSupportedException)
            {
                results.Add($"{Path.GetFileName(filepath)} Unsupported File Type ✘");
            }
            catch (Exception ex)
            {
                results.Add($"{Path.GetFileName(filepath)} Failed: {ex.Message} ✘");
                
            }
        }
        _context.SaveChanges();
        return results;
    }
    
    /*
    public List<string> AddMediaThroughFolderPaths(string folderPath)
    {
        
    }
    */
    
    // Removing list of chosen media
    
   
    public string RemoveMedia(int mediaId)
    {
        var media = _context.Medias.FirstOrDefault(x => x.Id == mediaId);

        if (media == null)
        {
            return "Media Not Found.";
        }

        try
        {
            bool fileDeleted = _storageManager.MediaFileExists(media.FilePath);

            _context.Medias.Remove(media);
            _context.SaveChanges();

            if (!fileDeleted)
            {
                return $"{media.Name} was removed from the database, but its physical file was already missing.";
            }

            return $"{media.Name} has been removed successfully.";
        }
        catch (IOException ex)
        {
            return $"Could not delete the media file: {ex.Message}";
        }
        catch (UnauthorizedAccessException ex)
        {
            return $"Access denied while deleting the media file: {ex.Message}";
        }
    }




    // Displaying all media
    public List<Media> DisplayAllMedia()
    {
        return _context.Medias.ToList();
    }

    // Displaying all videos
    public List<Media> DisplayAllVideos()
    {
        return _context.Medias.Where(x => x.Type == MediaType.Video).ToList();
    }

    // Displaying all audio
    public List<Media> DisplayAllAudio()
    {
        return _context.Medias.Where(x => x.Type == MediaType.Audio).ToList();
    }

    // Searching media by title
    public List<Media> SearchMediaByTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return DisplayAllMedia();

        title = title.Trim();
        
        return _context.Medias.Where(m => m.Name.Contains(title)).ToList();
    }
    
    private MediaType GetMediaType(string filepath)
    {
        string extension =
            Path.GetExtension(filepath).ToLowerInvariant();

        return extension switch
        {
            ".mp3" or ".wav" or ".flac" or ".aac"
                or ".m4a" or ".wma" or ".ogg" or ".opus"
                or ".aiff" or ".aif" => MediaType.Audio,

            ".mp4" or ".mkv" or ".avi" or ".mov" 
                or ".wmv" or ".webm" or ".m4v" => MediaType.Video,

            _ => throw new NotSupportedException()
        };
    }
}
