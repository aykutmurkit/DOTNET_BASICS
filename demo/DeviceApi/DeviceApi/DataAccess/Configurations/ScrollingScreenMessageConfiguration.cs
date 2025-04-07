using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// ScrollingScreenMessage entity konfigürasyonu
    /// </summary>
    public class ScrollingScreenMessageConfiguration : IEntityTypeConfiguration<ScrollingScreenMessage>
    {
        public void Configure(EntityTypeBuilder<ScrollingScreenMessage> builder)
        {
            // Temel ayarlar
            builder.ToTable("ScrollingScreenMessages");
            builder.HasKey(s => s.Id);
            
            // Id alanı yapılandırması - auto increment
            builder.Property(s => s.Id)
                  .UseIdentityColumn();
            
            // Türkçe ve İngilizce metin alanları
            builder.Property(s => s.TurkishLine).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(s => s.EnglishLine).HasColumnType("nvarchar(max)").IsRequired();
            
            // Zaman alanları
            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.UpdatedAt).IsRequired(false);
            
            // Cihaz ilişkisi
            builder.Property(s => s.DeviceId).IsRequired();
            
            // Device ile one-to-one ilişki
            builder.HasOne(s => s.Device)
                .WithOne(d => d.ScrollingScreenMessage)
                .HasForeignKey<ScrollingScreenMessage>(s => s.DeviceId)
                .OnDelete(DeleteBehavior.Cascade); // Device silinince ScrollingScreenMessage de silinecek
        }
    }
} 