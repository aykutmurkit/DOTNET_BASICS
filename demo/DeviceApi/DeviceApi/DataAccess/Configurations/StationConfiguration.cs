using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// Station entity için yapılandırma sınıfı
    /// </summary>
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            // Primary Key
            builder.HasKey(s => s.Id);
            
            // Id alanı yapılandırması - auto increment
            builder.Property(s => s.Id)
                  .UseIdentityColumn();
            
            // Diğer alan yapılandırmaları
            builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
            builder.Property(s => s.Latitude).IsRequired();
            builder.Property(s => s.Longitude).IsRequired();
            
            // İlişkiler
            builder.HasMany(s => s.Platforms)
                .WithOne(p => p.Station)
                .HasForeignKey(p => p.StationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 