
using Microsoft.EntityFrameworkCore;
using Vision_Media_Player.models;
using Vision_Media_Player.Data;

namespace Vision_Media_Player.Services;

public class MediaManager
{
    private readonly AppDbContext _context;
    private readonly MediaStorageManager _storageManager;

    public MediaManager(
        AppDbContext context,
        MediaStorageManager storageManager)
    {
        _context = context;
        _storageManager = storageManager;
    }

    public List<string> AddMediaThroughFilePath(List<string> filePaths)
    {
        var results = new List<string>();
        var copiedFiles = new List<string>();
        var importedMedia = new List<Media>();

        foreach (string filePath in filePaths)
        {
            if (!File.Exists(filePath))
            {
                results.Add($"{filePath} Not Found");
                continue;
            }

            try
            {
                MediaType mediaType = GetMediaType(filePath);

                // Read the actual duration before importing the file.
                TimeSpan duration = GetMediaDuration(filePath);

                string copiedFilePath =
                    _storageManager.CopyMediaFile(filePath, mediaType);

                copiedFiles.Add(copiedFilePath);

                string relativePath =
                    _storageManager.GetRelativePath(copiedFilePath);

                var media = new Media
                {
                    Name = Path.GetFileNameWithoutExtension(filePath),
                    Type = mediaType,
                    FilePath = relativePath,
                    Duration = duration,
                    CreatedAt = DateTime.Now
                };

                importedMedia.Add(media);

                results.Add(
                    $"{Path.GetFileName(filePath)} Ready to Import");
            }
            catch (NotSupportedException)
            {
                results.Add(
                    $"{Path.GetFileName(filePath)} Unsupported File Type");
            }
            catch (Exception ex)
            {
                results.Add(
                    $"{Path.GetFileName(filePath)} Failed: {ex.Message}");
            }
        }

        try
        {
            _context.Medias.AddRange(importedMedia);
            _context.SaveChanges();

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].EndsWith("Ready to Import"))
                {
                    results[i] = results[i].Replace(
                        "Ready to Import",
                        "Added Successfully");
                }
            }
        }
        catch (Exception ex)
        {
            foreach (string copiedFile in copiedFiles)
            {
                try
                {
                    if (File.Exists(copiedFile))
                        File.Delete(copiedFile);
                }
                catch
                {
                    // Cleanup failure should not hide the database error.
                }
            }

            _context.ChangeTracker.Clear();

            results.Add(
                $"Database import failed: {ex.Message}");
        }

        return results;
    }

    private TimeSpan GetMediaDuration(string filePath)
    {
        try
        {
            using var mediaFile = TagLib.File.Create(filePath);

            TimeSpan duration = mediaFile.Properties.Duration;

            return duration >= TimeSpan.Zero
                ? duration
                : TimeSpan.Zero;
        }
        catch
        {
            // Return zero if the format or its metadata is unsupported.
            return TimeSpan.Zero;
        }
    }

    public string RemoveMedia(int mediaId)
    {
        var media = _context.Medias
            .FirstOrDefault(x => x.Id == mediaId);

        if (media == null)
            return "Media Not Found.";

        string? trashFilePath = null;
        bool databaseDeleted = false;

        try
        {
            string fullPath = _storageManager.GetFullPath(
                media.FilePath);

            if (File.Exists(fullPath))
            {
                trashFilePath =
                    _storageManager.MoveMediaToTrash(media.FilePath);
            }

            _context.Medias.Remove(media);
            _context.SaveChanges();

            databaseDeleted = true;

            if (trashFilePath != null)
            {
                _storageManager.PermanentlyDeleteTrashFile(
                    trashFilePath);
            }

            return $"{media.Name} has been removed successfully.";
        }
        catch (Exception ex)
        {
            _context.ChangeTracker.Clear();

            if (!databaseDeleted && trashFilePath != null)
            {
                try
                {
                    _storageManager.RestoreMediaFromTrash(
                        trashFilePath,
                        media.FilePath);
                }
                catch (Exception restoreException)
                {
                    return
                        $"Database operation failed: {ex.Message}\n" +
                        $"File restoration also failed: {restoreException.Message}";
                }
            }

            return $"Could not remove media: {ex.Message}";
        }
    }

    public List<Media> DisplayAllMedia()
    {
        return _context.Medias
            .AsNoTracking()
            .ToList();
    }

    public List<Media> DisplayAllVideos()
    {
        return _context.Medias
            .AsNoTracking()
            .Where(x => x.Type == MediaType.Video)
            .ToList();
    }

    public List<Media> DisplayAllAudio()
    {
        return _context.Medias
            .AsNoTracking()
            .Where(x => x.Type == MediaType.Audio)
            .ToList();
    }

    public List<Media> SearchMediaByTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return DisplayAllMedia();

        title = title.Trim();

        return _context.Medias
            .AsNoTracking()
            .Where(m => m.Name.Contains(title))
            .ToList();
    }

    private MediaType GetMediaType(string filePath)
    {
        string extension =
            Path.GetExtension(filePath).ToLowerInvariant();

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
