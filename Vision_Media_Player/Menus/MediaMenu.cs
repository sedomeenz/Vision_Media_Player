
using Vision_Media_Player.Services;

namespace Vision_Media_Player.Menus;

public class MediaMenu
{
    private readonly MediaManager _mediaManager;
    private readonly MediaFileManager _mediaFileManager;

    public MediaMenu(MediaManager mediaManager, MediaFileManager mediaFileManager)
    {
        _mediaManager = mediaManager;
        _mediaFileManager = mediaFileManager;
    }

    public void ShowMediaMenu()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("===== Media =====");

            ShowAllMediaFiles();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Add single media                 add -s <filepath>");
            Console.WriteLine("Add multi media from folder      add -m <folderpath>");
            Console.WriteLine("Play media                       play <filename>");
            Console.WriteLine("Delete media                     dlt <filename1> <filename2> <filename3>");
            Console.WriteLine("Search media                     srch -n <filename>");
            Console.WriteLine("Back to main menu                >");
            Console.WriteLine();

            Console.Write("Run > ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string command = parts[0].ToLower();

            switch (command)
            {
                case "add":

                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: add -s <filepath>");
                        Console.ReadKey();
                        break;
                    }

                    if (parts[1].ToLower() == "-s")
                    {
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Usage: add -s <filepath>");
                            Console.ReadKey();
                            break;
                        }

                        var filePaths = new List<string>();

                        for (int i = 2; i < parts.Length; i++)
                        {
                            filePaths.Add(parts[i]);
                        }

                        var finalResults =
                            _mediaManager.AddMediaThroughFilePath(filePaths);

                        foreach (var result in finalResults)
                        {
                            Console.WriteLine(result);
                        }
                    }

                    break;

                case "play":
                    break;

                case "dlt":

                    if (parts.Length < 2)
                    {
                        Console.WriteLine(
                            "Usage: dlt <filename1> <filename2> <filename3>");
                        Console.ReadKey();
                        break;
                    }

                    var filenames = new List<string>();

                    for (int i = 1; i < parts.Length; i++)
                    {
                        filenames.Add(parts[i]);
                    }

                    var finalResult = _mediaManager.RemoveMedia(filenames);

                    foreach (var result in finalResult)
                    {
                        Console.WriteLine(result);
                    }

                    break;

                case "srch":
                    break;

                case ">":
                    return;

                default:
                    Console.WriteLine(
                        $"The command '{input}' was not recognized.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void ShowAllMediaFiles()
    {
        Console.WriteLine();
        Console.WriteLine("ALL YOUR MEDIA:");

        var allMedia = _mediaManager.DisplayAllMedia();
        var allSongs = _mediaManager.DisplayAllSongs();
        var allMovies = _mediaManager.DisplayAllMovies();

        if (allMedia.Count == 0)
        {
            Console.WriteLine("You don't have any media yet.");
            return;
        }

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
}

