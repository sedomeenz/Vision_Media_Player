namespace Vision_Media_Player.Helpers;

public class ConsoleHelper
{
    public static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    } 
}