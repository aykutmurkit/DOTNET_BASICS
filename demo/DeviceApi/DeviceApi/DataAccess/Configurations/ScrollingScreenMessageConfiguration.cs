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
            
            // Many-to-One ilişki (bir mesajı birden fazla cihaz kullanabilir)
            builder.HasMany(s => s.Devices)
                .WithOne(d => d.ScrollingScreenMessage)
                .HasForeignKey(d => d.ScrollingScreenMessageId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
} 