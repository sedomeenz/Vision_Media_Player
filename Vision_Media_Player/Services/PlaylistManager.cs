using Microsoft.EntityFrameworkCore;
using Vision_Media_Player.models;
using Vision_Media_Player.Data;

namespace Vision_Media_Player.Services;

public class PlaylistManager
{
    private readonly AppDbContext _context;
    
    public PlaylistManager(AppDbContext context)
    {
        _context = context;
    }
    
    // Creating a new playlist
    public bool CreateNewPlaylist(Playlist playlist)
    {
        _context.Playlists.Add(playlist);
        
        return _context.SaveChanges() > 0;
    }
    
    // Deleting a playlist
    public bool DeletePlaylist(Playlist playlist)
    {
        var existPlaylist = _context.Playlists.FirstOrDefault(p => p.Id == playlist.Id);       
        
        if (existPlaylist == null)
            return false;
        
        _context.Playlists.Remove(existPlaylist);
        
        return _context.SaveChanges() > 0;
    }

    // Adding media to the playlist
    public bool AddMediaToPlaylist(Playlist playlist, Media media)
    {
        bool playlistExist = _context.Playlists.Any(p => p.Id == playlist.Id);
        
        bool mediaExist = _context.Medias.Any(m => m.Id == media.Id);
        
        if (!playlistExist || !mediaExist)
            return false;
        
        bool alreadyExist = _context.PlaylistMedia.Any(pm => pm.PlaylistId == playlist.Id && pm.MediaId == media.Id);
        
        if (alreadyExist)
            return false;
        
        var playlistMedia = new PlaylistMedia(){PlaylistId =  playlist.Id, MediaId = media.Id};
        _context.PlaylistMedia.Add(playlistMedia);
        return _context.SaveChanges() > 0;
    }
    
    // Deleting media from the playlist
    public bool DeleteMediaFromPlaylist(Playlist playlist, Media media)
    {
        var playlistMedia = _context.PlaylistMedia.FirstOrDefault(x => x.PlaylistId == playlist.Id && x.MediaId == media.Id);
        
        if (playlistMedia == null)
            return false;
        
        _context.PlaylistMedia.Remove(playlistMedia);
        _context.SaveChanges();
        
        return true;
    }
    
    // Displaying all the playlist 
    public List<Playlist> DisplayAllPlaylists()
    {
        return _context.Playlists
            .Include(p => p.PlaylistMedias)
            .ThenInclude(pm => pm.Media)
            .ToList();
    }
    
    // Renaming the playlist name 
    public bool RenameThePlaylist(Playlist playlist, string name)
    {
       playlist.Name = name;
       _context.Playlists.Update(playlist); 
       _context.SaveChanges();
    }
    
    // Searching a playlist by name
    public List<Playlist> SearchPlaylistsByTitle(string title)
    {
       
    }  
}