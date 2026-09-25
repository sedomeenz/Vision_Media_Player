using Vision_Media_Player.Services;

namespace Vision_Media_Player.Menus;

public class SettingMenu
{
    private readonly UserManager _userManager;

    public SettingMenu(UserManager userManager)
    {
        _userManager = userManager;
    }
    
    public void ShowSettingMenu()
    {
        while (true)
        {
            Console.Clear();
                        
            Console.WriteLine("===== Setting =====");
            Console.WriteLine();
            Console.WriteLine("1. Profile");
            Console.WriteLine("2. Edit Profile");
            Console.WriteLine("3. About Us");
            Console.WriteLine("0. Back");
            Console.WriteLine();
                        
            Console.Write("Choose an option:");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Show the panel
                    break;
                
                case "2":
                    // Edit information 
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
}