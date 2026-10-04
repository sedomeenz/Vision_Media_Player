using Microsoft.EntityFrameworkCore;
using Vision_Media_Player.models;
using Vision_Media_Player.Data;
using Vision_Media_Player.Services;

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
    public List<string> RemoveMedia(List<string> filenames)
    {
     
        var results = new List<string>();

        foreach (var filename in filenames)
        {
            var media = _context.Medias.FirstOrDefault(x => Path.GetFileName(x.FilePath).Equals(filename, StringComparison.OrdinalIgnoreCase));
            
            if (media == null)
            {
                results.Add($"{filename} Not Found ✘");
                continue;
            }
            
            _storageManager.DeleteMediaFile(media.FilePath);
            _context.Medias.Remove(media);
            results.Add($"{filename} Has Been Removed Successfully ✔");
        }
        _context.SaveChanges();
        return results;
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
        return _context.Medias.Where(x => x.Name.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    
    private MediaType GetMediaType(string filepath)
    {
        string extension =
            Path.GetExtension(filepath).ToLowerInvariant();

        return extension switch
        {
            ".mp3" or ".wav" or ".flac" or ".aac"
                => MediaType.Audio,

            ".mp4" or ".mkv" or ".avi" or ".mov"
                => MediaType.Video,

            _ => throw new NotSupportedException()
        };
    }
}
