using System;
using Vision_Media_Player.Menus;
using Vision_Media_Player.Services;

namespace Vision_Media_Player
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var mediaManager = new MediaManager();
            var playlistManager = new PlaylistManager();
            var mediaFileService = new MediaFileManager();
            var userManager = new UserManager();
            
            var mainMenu = new MainMenu(mediaManager, playlistManager, userManager, mediaFileService);
            mainMenu.ShowMainMenu();
            
        }
    }
}