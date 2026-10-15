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

            Console.WriteLine("===== Playlists =====");
            
            ShowAllPlaylists();
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Create A New Playlist                 create ");
            Console.WriteLine("Delete Playlist                 ");
            Console.WriteLine("Show All Playlists                 ");
            Console.WriteLine("Add Media to Playlist                 ");
            Console.WriteLine("Remove Media from Playlist                 ");
            Console.WriteLine("Back");

            Console.ResetColor();
            Console.WriteLine();

            Console.Write("Choose an option:");
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
                    // Add Media
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