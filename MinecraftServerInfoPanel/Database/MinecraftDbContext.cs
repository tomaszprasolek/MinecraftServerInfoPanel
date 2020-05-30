using Microsoft.EntityFrameworkCore;

namespace MinecraftServerInfoPanel.Database
{
    public class MinecraftDbContext : DbContext
    {
        public MinecraftDbContext(DbContextOptions<MinecraftDbContext> options)
            : base(options)
        {

        }

        public DbSet<DbConsoleLog> ConsoleLogs { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<ServerUser> ServerUsers { get; set; }
    }
}