
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
    public bool CreateNewPlaylist(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var playlist = new Playlist
        {
            Name = name.Trim(),
        };

        _context.Playlists.Add(playlist);

        return _context.SaveChanges() > 0;
    }

   // Adds media to a playlist if it is not already included
    public bool AddMediaToPlaylist(int playlistId, int mediaId)
    {
        bool playlistExists = _context.Playlists
            .Any(p => p.Id == playlistId);

        bool mediaExists = _context.Medias
            .Any(m => m.Id == mediaId);

        if (!playlistExists || !mediaExists)
            return false;

        bool alreadyExists = _context.PlaylistMedia
            .Any(pm => pm.PlaylistId == playlistId &&
                       pm.MediaId == mediaId);

        if (alreadyExists)
            return false;

        var playlistMedia = new PlaylistMedia
        {
            PlaylistId = playlistId,
            MediaId = mediaId
        };

        _context.PlaylistMedia.Add(playlistMedia);

        return _context.SaveChanges() > 0;
    }

    // Removes media from a playlist without deleting the media itself
    public bool DeleteMediaFromPlaylist(int playlistId, int mediaId)
    {
        var playlistMedia = _context.PlaylistMedia
            .FirstOrDefault(pm =>
                pm.PlaylistId == playlistId &&
                pm.MediaId == mediaId);

        if (playlistMedia == null)
            return false;

        _context.PlaylistMedia.Remove(playlistMedia);

        return _context.SaveChanges() > 0;
    }

    // Displaying all playlists and their media
    public List<Playlist> DisplayAllPlaylists()
    {
        return _context.Playlists
            .AsNoTracking()
            .Include(p => p.PlaylistMedias)
            .ThenInclude(pm => pm.Media)
            .ToList();
    }

    // Deletes a playlist and its media relationships
    public Playlist? DisplayPlaylist(int playlistId)
    {
        return _context.Playlists
            .AsNoTracking()
            .Include(p => p.PlaylistMedias)
            .ThenInclude(pm => pm.Media)
            .FirstOrDefault(p => p.Id == playlistId);
    }

    // Deleting a playlist
    public bool DeletePlaylist(int playlistId)
    {
        var playlist = _context.Playlists
            .FirstOrDefault(p => p.Id == playlistId);

        if (playlist == null)
            return false;

        var playlistMedias = _context.PlaylistMedia
            .Where(pm => pm.PlaylistId == playlistId)
            .ToList();

        _context.PlaylistMedia.RemoveRange(playlistMedias);

        _context.Playlists.Remove(playlist);

        return _context.SaveChanges() > 0;
    }

    // Renaming a playlist
    public bool RenameThePlaylist(int playlistId, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return false;

        var playlist = _context.Playlists
            .FirstOrDefault(p => p.Id == playlistId);

        if (playlist == null)
            return false;

        newName = newName.Trim();

        if (playlist.Name == newName)
            return true;

        playlist.Name = newName;

        return _context.SaveChanges() > 0;
    }

    // Searching playlists by name
    public List<Playlist> SearchPlaylistsByTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return new List<Playlist>();

        string searchTerm = title.Trim().ToLower();

        return _context.Playlists
            .AsNoTracking()
            .Where(p => p.Name.ToLower().Contains(searchTerm))
            .Include(p => p.PlaylistMedias)
            .ThenInclude(pm => pm.Media)
            .ToList();
    }
}
