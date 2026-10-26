using Vision_Media_Player.Services;

namespace Vision_Media_Player.Menus;

public class PlaylistMenu
{
    private readonly MediaStorageManager _mediaStorageManager;
    private readonly PlaylistManager _playlistManager;
    
    public PlaylistMenu(MediaStorageManager mediaStorageManager, PlaylistManager playlistManager)
    {
        _mediaStorageManager = mediaStorageManager;
        _playlistManager = playlistManager;
    }

    public void ShowPlaylistMenu(PlaylistManager playlistManager)
    {
        while (true)
        {
            Console.Clear();

            var allPLaylist = _playlistManager.DisplayAllPlaylists();

            Console.WriteLine("==================================");
            Console.WriteLine("    PLAYLIST LIBRARY LIBRARY");
            Console.WriteLine("==================================");
            Console.WriteLine();
            Console.WriteLine($"Total Playlist: {allPLaylist.Count}");
            Console.WriteLine();
            Console.WriteLine("1. Create A New Playlist");
            Console.WriteLine("2. Delete Playlist");
            Console.WriteLine("3. Show All Playlists");
            Console.WriteLine("4. Add Media to Playlist");
            Console.WriteLine("5. Remove Media from Playlist");
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Choose an option: ");
            
            string? choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    // Create Playlist
                    break;

                case "2":
                    // Delete Playlist
                    break;

                case "3":
                    // Show Playlists
                    break;

                case "4":
                    // Add Media To The Playlist
                    break;

                case "5":
                    // Remove Media
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void ShowAllPlaylists()
    {
        
        
    }
    
    
}