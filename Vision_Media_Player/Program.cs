using Microsoft.EntityFrameworkCore;
using Vision_Media_Player.Data;
using Vision_Media_Player.Menus;
using Vision_Media_Player.Services;

namespace Vision_Media_Player
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=vision_media_player.db")
                .Options;

            using var context = new AppDbContext(options);
            context.Database.Migrate();
            
            var storageManager = new MediaStorageManager();
            var mediaManager = new MediaManager(context, storageManager);
            var playlistManager = new PlaylistManager(context);
            var userManager = new UserManager(context);
            
            var mainMenu = new MainMenu(
                mediaManager,
                playlistManager,
                userManager,
                storageManager);
            
            var currentUser = userManager.GetCurrentUser();

            if (currentUser == null)
            {
                // First Run Setup

                string userName;
                string email;
                
                while (true)
                {
                    Console.WriteLine("Welcome to Vision Media Player!");
                    Console.WriteLine("Let's setup your profile.\n");
                
                    Console.Write("Enter your name: ");
                    userName = Console.ReadLine()?.Trim() ?? "";
                
                    Console.Write("Enter your email ");
                    email = Console.ReadLine()?.Trim() ?? "";

                    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(email))
                    {
                        break;
                    }
                    
                    Console.WriteLine("Name and email cannot be empty. try  again.\n");
                }
                
                currentUser = userManager.CreateUser(email, userName);
                Console.WriteLine($"\nWelcome, {currentUser.Name}!");
                
                mainMenu.ShowMainMenu();
            }
            
            mainMenu.ShowMainMenu();
        }
    }
}
