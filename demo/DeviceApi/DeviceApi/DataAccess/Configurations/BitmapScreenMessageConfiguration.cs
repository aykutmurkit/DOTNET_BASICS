using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// BitmapScreenMessage entity konfigürasyonu
    /// </summary>
    public class BitmapScreenMessageConfiguration : IEntityTypeConfiguration<BitmapScreenMessage>
    {
        public void Configure(EntityTypeBuilder<BitmapScreenMessage> builder)
        {
            // Temel ayarlar
            builder.ToTable("BitmapScreenMessages");
            builder.HasKey(b => b.Id);
            
            // Id alanı yapılandırması - auto increment
            builder.Property(b => b.Id)
                  .UseIdentityColumn();
            
            // Türkçe ve İngilizce bitmap alanları
            builder.Property(b => b.TurkishBitmap).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(b => b.EnglishBitmap).HasColumnType("nvarchar(max)").IsRequired();
            
            // Zaman alanları
            builder.Property(b => b.CreatedAt).IsRequired();
            builder.Property(b => b.UpdatedAt).IsRequired(false);
            
            // Cihaz ilişkisi
            builder.Property(b => b.DeviceId).IsRequired();
            
            // Device ile one-to-one ilişki
            builder.HasOne(b => b.Device)
                .WithOne(d => d.BitmapScreenMessage)
                .HasForeignKey<BitmapScreenMessage>(b => b.DeviceId)
                .OnDelete(DeleteBehavior.Cascade); // Device silinince BitmapScreenMessage de silinecek
        }
    }
} 