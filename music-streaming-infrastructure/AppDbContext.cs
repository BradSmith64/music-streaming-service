using Microsoft.EntityFrameworkCore;

using music_streaming_infrastructure.Persistence;

namespace music_streaming_infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Song> Songs { get; set; }
    public DbSet<Like> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Like>()
            .HasIndex(l => l.SongId);
    }
}