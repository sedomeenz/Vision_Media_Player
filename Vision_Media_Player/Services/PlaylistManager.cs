using Vision_Media_Player.models;

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
        playlist.PlaylistMedias.Add(new PlaylistMedia
        {
            PlaylistId = playlist.Id,
            MediaId = media.Id,
            Playlist =  playlist,
            Media = media
        });
    }
    
    // Deleting media from the playlist
    public void DeleteMediaFromPlaylist(Playlist playlist, Media media)
    {
        var playlistMedia = playlist.PlaylistMedias.FirstOrDefault(pm => pm.MediaId == media.Id);

        if (playlistMedia != null)
        {
            playlist.PlaylistMedias.Remove(playlistMedia);
        }
    }
    
    // Displaying all the playlist 
    public List<Playlist> DisplayAllPlaylists()
    {
        return _playlists;
    }
    
    // Renaming the playlist name 
    public void RenameThePlaylist(Playlist playlist, string name)
    {
        playlist.Name = name;
    }
    
    // Searching a playlist by name
    public List<Playlist> SearchPlaylistsByTitle(string title)
    {
        var result = new List<Playlist>();
        foreach (var playlist in _playlists)
        {
            if (playlist.Name.Contains(title, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(playlist);
            }
        }
        return result;
    }  
}