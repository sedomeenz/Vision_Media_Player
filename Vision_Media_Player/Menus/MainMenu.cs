using Vision_Media_Player.Services;

namespace Vision_Media_Player.Menus;

public class MainMenu
{
    private readonly MediaManager _mediaManager;
    private readonly PlaylistManager _playlistManager;
    private readonly UserManager _userManager;
    private readonly MediaFileManager _mediaFileManager;

    public MainMenu(
        MediaManager mediaManager,
        PlaylistManager playlistManager,
        UserManager userManager,
        MediaFileManager mediaFileManager)
    {
        _mediaManager = mediaManager;
        _playlistManager = playlistManager;
        _userManager = userManager;
        mediaFileManager = mediaFileManager;
    }

    public void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("Vision Media Player");
            Console.WriteLine();
            // Console.WriteLine("  _   ___     _             __  ___       ___        ___  __                 \n | | / (_)__ (_)__  ___    /  |/  /__ ___/ (_)__ _  / _ \\/ /__ ___ _____ ____\n | |/ / (_-</ / _ \\/ _ \\  / /|_/ / -_) _  / / _ `/ / ___/ / _ `/ // / -_) __/\n |___/_/___/_/\\___/_//_/ /_/  /_/\\__/\\_,_/_/\\_,_/ /_/  /_/\\_,_/\\_, /\\__/_/   \n                                                              /___/          ");

            Console.WriteLine("Welcom to Vision Media Player");

            Console.WriteLine("1. Media");
            Console.WriteLine("2. Playlists");
            Console.WriteLine("3. Users");
            Console.WriteLine("4. Help");
            Console.WriteLine("0. Exit");

            Console.ResetColor();
            Console.WriteLine();

            Console.Write("Choose an option:");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Media menu
                    var mediaMenu = new MediaMenu(_mediaManager, _mediaFileManager);
                    mediaMenu.ShowMediaMenu();
                    break;

                case "2":
                    // Playlist menu
                    var playlistMenu = new PlaylistMenu(_playlistManager);
                    break;

                case "3":
                    // Setting menu
                    var settingMenu = new SettingMenu(_userManager);
                    break;

                case "4":
                    // Help menu
                    ShowHelpMenu();
                    break;

                case "0":
                    // Exite the program
                    Console.WriteLine("good bye.");
                    return;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option");
                    Console.ReadKey();
                    break; 
            }
        }
    }
    
    private void ShowHelpMenu()
    {
        
        Console.Clear();

        Console.WriteLine("===== Help Page =====");
        Console.WriteLine("bella bella bella");
        Console.WriteLine("bellaaa bllaa");

        Console.WriteLine("\nPress any key to go back...");
        Console.ReadKey();
    }
}