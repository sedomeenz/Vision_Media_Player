
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
    public DbSet<PlaylistMedia> PlaylistMedia { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PlaylistMedia composite primary key
        modelBuilder.Entity<PlaylistMedia>()
            .HasKey(pm => new { pm.PlaylistId, pm.MediaId });

        // Playlist -> PlaylistMedia
        modelBuilder.Entity<PlaylistMedia>()
            .HasOne(pm => pm.Playlist)
            .WithMany(p => p.PlaylistMedias)
            .HasForeignKey(pm => pm.PlaylistId);

        // Media -> PlaylistMedia
        modelBuilder.Entity<PlaylistMedia>()
            .HasOne(pm => pm.Media)
            .WithMany(m => m.PlaylistMedias)
            .HasForeignKey(pm => pm.MediaId);

        // User -> Playlist
        modelBuilder.Entity<Playlist>()
            .HasOne(p => p.User)
            .WithMany(u => u.Playlists)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
