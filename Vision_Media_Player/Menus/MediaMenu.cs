using Vision_Media_Player.Services;

namespace Vision_Media_Player.Menus;
// hello
public class MediaMenu
{
    private readonly MediaManager _mediaManager;
    private readonly MediaFileManager _mediaFileManager;
    
    public MediaMenu(MediaManager mediaManager, MediaFileManager mediaFileManager)
    {
        _mediaManager = mediaManager;
        _mediaFileManager = mediaFileManager; //
    }
    
    public void ShowMediaMenu()
    {
        while (true)
        {
            Console.Clear();
                        
            Console.WriteLine("===== Media =====");
            Console.WriteLine();
            Console.WriteLine("ALL YOUR MEDIA:"); //jkljsfd  jdlsfdsfdsfdsfsdfdfdsfdsf

            var allmedia = _mediaManager.DisplayAllMedia();    
            var allSongs = _mediaManager.DisplayAllSongs();
            var allMovies = _mediaManager.DisplayAllMovies();

            if (allmedia.Count == 0)
            {
                Console.WriteLine("You don't have any media yet.");
            }
            else
            {
                if (allSongs.Count > 0)
                {
                    Console.WriteLine("===== All Songs =====");

                    for (int i = 0; i < allSongs.Count; i++)
                    {
                        Console.WriteLine(
                            $"{i + 1}. File name: {allSongs[i].Title} | Type: Song");
                    }
                }

                if (allMovies.Count > 0)
                {
                    Console.WriteLine("===== All Movies =====");

                    for (int i = 0; i < allMovies.Count; i++)
                    {
                        Console.WriteLine(
                            $"{i + 1}. File name: {allMovies[i].Title} | Type: Movie");
                    }
                }
            }
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("1. Add Media");
            Console.WriteLine("2. Play media");
            Console.WriteLine("3. Delete Media");
            Console.WriteLine("4. Search Media");
            Console.WriteLine("0. Back");
            Console.WriteLine();
                        
            Console.Write("Choose an option:");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Add media
                    ShowAddMediaMenu();
                    break;
                
                case "2":
                    // Delete media
                    Console.Clear();
                    Console.Write("Enter the path of the file you want to add: ");
                    break;
                
                case "3":
                    // Search media
                    break;
                
                case "4":
                    // Show all media
                    
                    if (true)
                    {
                        
                    }
                    
                    break;
                
                case "5":
                    // Show all movies
                    break;
                
                case "6":
                    //Show all songs
                    break;
                
                case "0":
                    // Back to the Main Menu
                    return;
                
                default:
                    Console.WriteLine();
                    Console.WriteLine("Please select a valid option.");
                    Console.ReadKey();
                    break;
            }
        }
    }
    
    private void ShowAddMediaMenu()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("===== Add Media =====");
            Console.WriteLine();
            Console.WriteLine("1. Add Single Media");
            Console.WriteLine("2. Add Media by Folder");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Add single media
                    Console.Clear();
                    Console.Write("Enter the path of the file you want to add: ");
                    var filePath = Console.ReadLine().Trim();
                    
                    break;

                case "2":
                    // Add by folder
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
}