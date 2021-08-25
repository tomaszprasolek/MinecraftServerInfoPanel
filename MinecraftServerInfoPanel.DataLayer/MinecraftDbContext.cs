using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinecraftServerInfoPanel.Domain.Entities;

namespace MinecraftServerInfoPanel.DataLayer
{
    public class MinecraftDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);

            var config = builder.Build();

            optionsBuilder.UseSqlServer(config["ConnectionString"]);
        }

        public DbSet<DbConsoleLog> ConsoleLogs { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<ServerUser> ServerUsers { get; set; }

        public DbSet<Email> Emails { get; set; }
    }
}