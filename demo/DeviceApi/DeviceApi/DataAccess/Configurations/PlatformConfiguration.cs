using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// Platform entity için yapılandırma sınıfı
    /// </summary>
    public class PlatformConfiguration : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            // Primary Key
            builder.HasKey(p => p.Id);
            
            // Id alanı yapılandırması - auto increment
            builder.Property(p => p.Id)
                  .UseIdentityColumn();
            
            // Diğer alan yapılandırmaları
            builder.Property(p => p.Latitude).IsRequired();
            builder.Property(p => p.Longitude).IsRequired();
            builder.Property(p => p.StationId).IsRequired();
            
            // İlişkiler
            builder.HasOne(p => p.Station)
                .WithMany(s => s.Platforms)
                .HasForeignKey(p => p.StationId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasMany(p => p.Devices)
                .WithOne(d => d.Platform)
                .HasForeignKey(d => d.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 