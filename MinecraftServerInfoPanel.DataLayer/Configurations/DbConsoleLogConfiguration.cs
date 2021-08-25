using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerInfoPanel.Domain.Entities;

namespace MinecraftServerInfoPanel.DataLayer.Configurations
{
    public class DbConsoleLogConfiguration : IEntityTypeConfiguration<DbConsoleLog>
    {
        public void Configure(EntityTypeBuilder<DbConsoleLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Information)
                .IsRequired();
        }
    }

}
