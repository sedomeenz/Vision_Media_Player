using Microsoft.EntityFrameworkCore;
using Vision_Media_Player.models;

namespace Vision_Media_Player.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Media> Medias { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
}