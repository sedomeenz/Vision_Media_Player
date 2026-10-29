using System.Windows;
using Microsoft.EntityFrameworkCore;
using Vision_Media_Player.Data;
using Vision_Media_Player.Services;

namespace Vision_Media_Player.UI;

public partial class App : Application
{
    public AppDbContext DbContext { get; private set; } = null!;
    public MediaStorageManager StorageManager { get; private set; } = null!;
    public MediaManager MediaManager { get; private set; } = null!;
    public PlaylistManager PlaylistManager { get; private set; } = null!;
    public UserManager UserManager { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(
                @"Data Source=D:\My stuff\Projects\Vision_Media_Player\Vision_Media_Player\vision_media_player.db")
            .Options;

        DbContext = new AppDbContext(options);
        DbContext.Database.Migrate();

        StorageManager = new MediaStorageManager();
        MediaManager = new MediaManager(DbContext, StorageManager);
        PlaylistManager = new PlaylistManager(DbContext);
        UserManager = new UserManager(DbContext);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        DbContext?.Dispose();
        base.OnExit(e);
    }
}