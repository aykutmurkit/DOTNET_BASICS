using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// Prediction entity'si için veritabanı konfigürasyonu
    /// </summary>
    public class PredictionConfiguration : IEntityTypeConfiguration<Prediction>
    {
        public void Configure(EntityTypeBuilder<Prediction> builder)
        {
            // Tablo adı
            builder.ToTable("Predictions");
            
            // Primary key
            builder.HasKey(p => p.Id);
            
            // Kolonlar için konfigürasyonlar
            builder.Property(p => p.StationName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Direction).IsRequired().HasMaxLength(50);
            
            // Nullable kolonlar
            builder.Property(p => p.Train1).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.Line1).HasMaxLength(20).IsRequired(false);
            builder.Property(p => p.Destination1).HasMaxLength(100).IsRequired(false);
            builder.Property(p => p.Time1).IsRequired(false);
            
            builder.Property(p => p.Train2).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.Line2).HasMaxLength(20).IsRequired(false);
            builder.Property(p => p.Destination2).HasMaxLength(100).IsRequired(false);
            builder.Property(p => p.Time2).IsRequired(false);
            
            builder.Property(p => p.Train3).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.Line3).HasMaxLength(20).IsRequired(false);
            builder.Property(p => p.Destination3).HasMaxLength(100).IsRequired(false);
            builder.Property(p => p.Time3).IsRequired(false);
            
            // Zorunlu alanlar
            builder.Property(p => p.ForecastGenerationAt).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.PlatformId).IsRequired();
            
            // Platform ile one-to-one ilişki
            builder.HasOne(p => p.Platform)
                .WithOne(p => p.Prediction)
                .HasForeignKey<Prediction>(p => p.PlatformId)
                .OnDelete(DeleteBehavior.Cascade); // Platform silinirse tahmin de silinsin
        }
    }
} 