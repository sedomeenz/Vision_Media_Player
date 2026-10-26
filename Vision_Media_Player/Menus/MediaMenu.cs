using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vision_Media_Player.Helpers;
using Vision_Media_Player.Services;



namespace Vision_Media_Player.Menus;

public class MediaMenu
{
    private readonly MediaManager _mediaManager;

    public MediaMenu(MediaManager mediaManager, MediaStorageManager mediaStorageManager)
    {
        _mediaManager = mediaManager;
    }

    public void ShowMediaMenu()
    {
        while (true)
        {
            Console.Clear();
            
            var allMedia = _mediaManager.DisplayAllMedia();
            
            Console.WriteLine("==================================");
            Console.WriteLine("         MEDIA LIBRARY");
            Console.WriteLine("==================================");
            Console.WriteLine();
            Console.WriteLine($"Total Media: {allMedia.Count}");
            Console.WriteLine();
            Console.WriteLine("1. Show All Media");
            Console.WriteLine("2. Add Media");
            Console.WriteLine("3. Delete Media");
            Console.WriteLine("4. Search Media");
            Console.WriteLine("0. Back to Main Menu");
            Console.WriteLine();
            Console.Write("Choose an option: ");
            
            string? choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    ShowAllMediaFiles();
                    break;

                case "2":
                    AddMedia();
                    break;

                case "3":
                    DeleteMedia();
                    break;

                case "4":
                    SearchMedia();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }

    private void ShowAllMediaFiles()
    {
        Console.Clear();

        Console.WriteLine("========== ALL MEDIA ==========");
        Console.WriteLine();

        ShowMediaList();
        ConsoleHelper.Pause();
    }
    
    private void ShowMediaList()
    {
        var allMedia = _mediaManager.DisplayAllMedia();

        Console.WriteLine("========== YOUR MEDIA ==========");
        Console.WriteLine();

        if (allMedia.Count == 0)
        {
            Console.WriteLine("You don't have any media yet.");
            return;
        }

        foreach (var media in allMedia)
        {
            Console.WriteLine(
                $"ID: {media.Id} | Name: {media.Name} | Type: {media.Type}");
        }
    }
    
    private void AddMedia()
    {
        Console.Clear();
        Console.WriteLine("========== ADD MEDIA ==========");
        Console.WriteLine();
        Console.WriteLine("Enter the full path of the media file.");
        Console.WriteLine("Example: C:\\Music\\Dream On.mp3");
        Console.WriteLine();
        Console.Write("File path (0 to cancel): ");

        string? filePath = Console.ReadLine()?.Trim().Trim('"');

        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("File path cannot be empty.");
            ConsoleHelper.Pause();
            return;
        }

        if (filePath == "0")
            return;
        
        var results = _mediaManager.AddMediaThroughFilePath(new List<string> { filePath });

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
        
        ConsoleHelper.Pause();
    }
    
    private void DeleteMedia()
    {
        Console.Clear();
        
        ShowMediaList();
        
        var allmedia = _mediaManager.DisplayAllMedia();

        if (allmedia.Count == 0)
        {
            ConsoleHelper.Pause();
            return;
        }
        
        Console.WriteLine();
        Console.Write("Enter the media ID to delete (0 to cancel): ");

        if (!int.TryParse(Console.ReadLine(), out int mediaId))
        {
            Console.WriteLine("Invalid media ID.");
            ConsoleHelper.Pause();
            return;
        }

        if (mediaId == 0)
            return;
        
        var media = allmedia.FirstOrDefault(m => m.Id == mediaId);

        if (media == null)
        {
            Console.WriteLine("Media not found.");
            ConsoleHelper.Pause();
            return;
        }
        
        Console.WriteLine($"Are you sure you want to delete \"{media.Name}\"? (y/n)");

        string? confirm = Console.ReadLine();

        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Deletion cancelled.");
            ConsoleHelper.Pause();
            return;
        }

        string filename = Path.GetFileName(media.FilePath);

        var results = _mediaManager.RemoveMedia(new List<string> { filename });

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
        
        ConsoleHelper.Pause();
    }
    
    private void SearchMedia()
    {
        Console.Clear();

        Console.WriteLine("========== SEARCH MEDIA ==========");
        Console.WriteLine();
        Console.Write("Enter a media title (0 to cancel): ");

        string? title = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Search title cannot be empty.");
            ConsoleHelper.Pause();            return;
        }

        if (title == "0")
            return;

        var results = _mediaManager.SearchMediaByTitle(title);

        Console.WriteLine();
        Console.WriteLine("========== SEARCH RESULTS ==========");
        Console.WriteLine();

        if (results.Count == 0)
        {
            Console.WriteLine("No matching media found.");
        }
        else
        {
            foreach (var media in results)
            {
                Console.WriteLine(
                    $"ID: {media.Id} | " +
                    $"Name: {media.Name} | " +
                    $"Type: {media.Type}");
            }
        }
        
        ConsoleHelper.Pause();
    }
}

