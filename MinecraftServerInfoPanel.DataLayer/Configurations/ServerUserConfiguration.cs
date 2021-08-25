using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerInfoPanel.Domain.Entities;

namespace MinecraftServerInfoPanel.DataLayer.Configurations
{
    public class ServerUserConfiguration : IEntityTypeConfiguration<ServerUser>
    {
        public void Configure(EntityTypeBuilder<ServerUser> builder)
        {
            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Xuid)
                .IsRequired()
                .HasMaxLength(16);
        }
    }

}
