
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Vision_Media_Player.Data;
using Vision_Media_Player.Services;
using LibVLCSharp.Shared;

namespace Vision_Media_Player.UI;

public partial class App : Application
{
    public AppDbContext DbContext { get; private set; } = null!;
    public MediaStorageManager StorageManager { get; private set; } = null!;
    public MediaManager MediaManager { get; private set; } = null!;
    public PlaylistManager PlaylistManager { get; private set; } = null!;
    public LibVLC LibVlc { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            Core.Initialize();
            LibVlc = new LibVLC();

            StorageManager = new MediaStorageManager();

            string dataDirectory = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "VisionMediaPlayer");

            Directory.CreateDirectory(dataDirectory);

            string databasePath = Path.Combine(
                dataDirectory,
                "vision_media_player.db");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            DbContext = new AppDbContext(options);

            DbContext.Database.Migrate();

            MediaManager = new MediaManager(
                DbContext,
                StorageManager);

            PlaylistManager = new PlaylistManager(DbContext);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"The application could not start.\n\n{ex.Message}",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        LibVlc?.Dispose();
        DbContext?.Dispose();

        base.OnExit(e);
    }
}
