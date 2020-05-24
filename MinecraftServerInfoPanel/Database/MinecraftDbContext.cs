using Microsoft.EntityFrameworkCore;

namespace MinecraftServerInfoPanel.Database
{
    public class MinecraftDbContext : DbContext
    {
        public MinecraftDbContext(DbContextOptions<MinecraftDbContext> options)
            : base(options)
        {

        }

        public DbSet<ConsoleLog> ConsoleLogs { get; set; }
    }
}