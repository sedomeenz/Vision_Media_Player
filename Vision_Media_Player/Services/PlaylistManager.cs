namespace Vision_Media_Player.Services;

public class PlaylistManager
{
    private readonly List<Playlist> _playlists = new();
    
    // Creating a new playlist
    public void CreateNewPlaylist(Playlist playlist)
    {
        _playlists.Add(playlist);
    }
    
    // Deleting a playlist
    public void DeletePlaylist(Playlist playlist)
    {
        _playlists.Remove(playlist);
    }

    // Adding media to the playlist
    public void AddMediaToPlaylist(Playlist playlist, Media media)
    {
        playlist.Items.Add(media);
    }
    
    // Deleting media from the playlist
    public void DeleteMediaFromPlaylist(Playlist playlist, Media media)
    {
        playlist.Items.Remove(media);
    }
    
    // Displaying all the playlist 
    public List<Playlist> DisplayAllPlaylists()
    {
        return _playlists;
    }
    
    // Renaming the playlist name 
    public void RenameThePlaylist(Playlist playlist, string title)
    {
        playlist.Title = title;
    }
    
    // Searching a playlist by name
    public List<Playlist> SearchPlaylistsByTitle(string title)
    {
        var result = new List<Playlist>();
        foreach (var playlist in _playlists)
        {
            if (playlist.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(playlist);
            }
        }
        return result;
    }  
}