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
            
            // Many-to-One ilişki (bir mesajı birden fazla cihaz kullanabilir)
            builder.HasMany(b => b.Devices)
                .WithOne(d => d.BitmapScreenMessage)
                .HasForeignKey(d => d.BitmapScreenMessageId)
                .OnDelete(DeleteBehavior.SetNull); // BitmapScreenMessage silinirse cihazların ilişkisi null olacak
        }
    }
} 