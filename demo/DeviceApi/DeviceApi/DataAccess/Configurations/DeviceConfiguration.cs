using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// Device entity için yapılandırma sınıfı
    /// </summary>
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            // Primary Key
            builder.HasKey(d => d.Id);
            
            // Id alanı yapılandırması - auto increment
            builder.Property(d => d.Id)
                  .UseIdentityColumn();
            
            // Diğer alan yapılandırmaları
            builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
            builder.Property(d => d.Ip).HasMaxLength(50).IsRequired();
            builder.Property(d => d.Port).IsRequired();
            builder.Property(d => d.Latitude).IsRequired();
            builder.Property(d => d.Longitude).IsRequired();
            builder.Property(d => d.PlatformId).IsRequired();
            builder.Property(d => d.FullScreenMessageId).IsRequired(false);
            builder.Property(d => d.ScrollingScreenMessageId).IsRequired(false);
            
            // İlişkiler
            builder.HasOne(d => d.Platform)
                .WithMany(p => p.Devices)
                .HasForeignKey(d => d.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // FullScreenMessage ile Many-to-One ilişki
            builder.HasOne(d => d.FullScreenMessage)
                .WithMany(f => f.Devices)
                .HasForeignKey(d => d.FullScreenMessageId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
                
            // ScrollingScreenMessage ile Many-to-One ilişki
            builder.HasOne(d => d.ScrollingScreenMessage)
                .WithMany(s => s.Devices)
                .HasForeignKey(d => d.ScrollingScreenMessageId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
} 