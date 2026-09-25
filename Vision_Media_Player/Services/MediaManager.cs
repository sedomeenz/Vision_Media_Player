using System.IO.Enumeration;

namespace Vision_Media_Player.Services;

public class MediaManager
{
    private readonly List<Media> _media = new();

    // Adding new media 
    public void AddMedia(Media media)
    {
        _media.Add(media);
    }
    
    /* Removing list of chosen media
     public List<string> RemoveMedia(List<string> mediaList)
    {
        var faild = new List<string>();

        foreach (var media in mediaList)
        {





        }
    }
    */

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