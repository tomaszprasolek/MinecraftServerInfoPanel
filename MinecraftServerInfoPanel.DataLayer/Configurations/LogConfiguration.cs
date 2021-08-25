using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerInfoPanel.Domain.Entities;

namespace MinecraftServerInfoPanel.DataLayer.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.MessageTemplate)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Level)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Exception)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Properties)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.LogEvent)
                .IsRequired()
                .HasMaxLength(200);
        }
    }

}
