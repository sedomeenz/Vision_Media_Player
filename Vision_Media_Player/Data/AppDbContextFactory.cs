using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;


namespace Vision_Media_Player.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlite("Data Source=vision_media_player.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}