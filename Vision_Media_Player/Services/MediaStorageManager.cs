using Vision_Media_Player.models;

namespace Vision_Media_Player.Services;

public class MediaStorageManager
{
    private readonly string _rootPath;
    
    public string MediaPath { get; }
    public string AudioFilesPath { get; }
    public string VideoFilesPath { get; }
    public string PlaylistPath { get; }
    
    public MediaStorageManager()
    {
        _rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VisionMediaPlayer");
        
        MediaPath = Path.Combine(_rootPath, "Media");
        AudioFilesPath = Path.Combine(_rootPath, "Audio");
        VideoFilesPath = Path.Combine(MediaPath, "Video");
        PlaylistPath = Path.Combine(_rootPath, "Playlist");
        
        CreateFolders();
    }
    
    private void CreateFolders()
    {
        Directory.CreateDirectory(_rootPath);
        Directory.CreateDirectory(MediaPath);
        Directory.CreateDirectory(AudioFilesPath);
        Directory.CreateDirectory(VideoFilesPath);
        Directory.CreateDirectory(PlaylistPath);
    }

    public string CopyMediaFile(string sourcePath, MediaType type)
    {
        string destinationFolder = type switch
        {
            MediaType.Audio => AudioFilesPath,
            MediaType.Video => VideoFilesPath, 
            _ => throw new NotSupportedException("Unsupported media type.")
        };

        string fileName = Path.GetFileName(sourcePath);
        string destinationPath = Path.Combine(destinationFolder, fileName);
        
        destinationPath = GetUniqueFilePath(destinationPath);
        File.Copy(sourcePath, destinationPath, true);
        
        return destinationPath;
    }

    public void DeleteMediaFile(string relativePath)
    {
        string fullPath = GetUniqueFilePath(relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public string GetRelativePath(string fullPath)
    {
        string relativePath = Path.GetRelativePath(_rootPath, fullPath);
        
        return relativePath.Replace('\\', '/');
    }

    public string GetFullPath(string relativePath)
    {
        string normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar);

        return  Path.Combine(_rootPath, normalizedPath);
    }
    
    public string CreatePlaylistFolder(string playlistName)
    {
        string safeName = MakeSafeFolderName(playlistName);

        string playlistPath = Path.Combine(PlaylistPath, safeName);

        Directory.CreateDirectory(playlistPath);

        return playlistPath;
    }

    
    private string GetUniqueFilePath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return filePath;
        }

        string directory = Path.GetDirectoryName(filePath)!;
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);

        int counter = 1;

        while (true)
        {
            string newFilePath = Path.Combine(
                directory,
                $"{fileName} ({counter}){extension}");

            if (!File.Exists(newFilePath))
            {
                return newFilePath;
            }

            counter++;
        }
    }

    private string MakeSafeFolderName(string name)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return name.Trim();
    }
}
