using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerInfoPanel.Domain.Entities;

namespace MinecraftServerInfoPanel.DataLayer.Configurations
{
    public class EmailConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.Property(x => x.EmailAddress)
                .IsRequired()
                .HasMaxLength(100);
        }
    }

}
