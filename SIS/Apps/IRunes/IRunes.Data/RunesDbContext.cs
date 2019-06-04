using IRunes.Models;

using Microsoft.EntityFrameworkCore;

namespace IRunes.Data
{
    public class RunesDbContext : DbContext
    {
        public RunesDbContext()
        { }

        public RunesDbContext(DbContextOptions<RunesDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseLazyLoadingProxies()
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(user =>
            {
                user.Property(u => u.Username)
                    .HasMaxLength(100);

                user.Property(u => u.Email)
                    .HasMaxLength(256);
            });

            builder.Entity<Album>(album =>
            {
                album.HasMany(a => a.Tracks)
                    .WithOne(t => t.Album)
                    .HasForeignKey(t => t.AlbumId);
            });
        }
    }
}