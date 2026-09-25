using System.IO.Enumeration;

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
            if (! _mediaFileManager.FileExists(filepath))
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

    //Removing list of chosen media
     public List<string> RemoveMedia(List<string> filenames)
    {
        
        var results = new List<string>(); 

        foreach (var filename in filenames)
        {
            var media = _media.FirstOrDefault(x => Path.GetFileName(x.FilePath).Equals(filename, StringComparison.OrdinalIgnoreCase));
            
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

    // Displaying list of media you have
    public List<Media> DisplayAllMedia()
    {
        return _media;
    } 
    
    // Displaying list of movies you have
    public List<Movie> DisplayAllMovies()
    {
        var listOfMovies = new List<Movie>();
        foreach (var media in _media)
        {
            if (media is Movie movie)
            {
                listOfMovies.Add(movie);
            }
        }
        return listOfMovies;
    }
    
    // Displaying list of all songs you have
    public List<Song> DisplayAllSongs()
    {
        var listOfSongs = new List<Song>();
        foreach (var media in _media)
        {
            if (media is Song song)
            {
                listOfSongs.Add(song);
            }
        }
        return listOfSongs;
    }
    
    // Searching a media 
    public List<Media> SearchMediaByTitle(string title)
    {
        List<Media> results =  null;

        foreach (var media in _media)
        {
            if (media.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(media); 
            }
        }
        return results;
    }
}