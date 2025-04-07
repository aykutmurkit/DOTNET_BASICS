using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceApi.DataAccess.Configurations
{
    /// <summary>
    /// FullScreenMessage entity konfigürasyonu
    /// </summary>
    public class FullScreenMessageConfiguration : IEntityTypeConfiguration<FullScreenMessage>
    {
        public void Configure(EntityTypeBuilder<FullScreenMessage> builder)
        {
            builder.ToTable("FullScreenMessages");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.TurkishLine1).HasMaxLength(255).IsRequired(false);
            builder.Property(f => f.TurkishLine2).HasMaxLength(255).IsRequired(false);
            builder.Property(f => f.TurkishLine3).HasMaxLength(255).IsRequired(false);
            builder.Property(f => f.TurkishLine4).HasMaxLength(255).IsRequired(false);

            builder.Property(f => f.EnglishLine1).HasMaxLength(255).IsRequired(false);
            builder.Property(f => f.EnglishLine2).HasMaxLength(255).IsRequired(false);
            builder.Property(f => f.EnglishLine3).HasMaxLength(255).IsRequired(false);
            builder.Property(f => f.EnglishLine4).HasMaxLength(255).IsRequired(false);

            builder.Property(f => f.CreatedAt).IsRequired();
            builder.Property(f => f.ModifiedAt).IsRequired(false);

            // One-to-One ilişki
            builder.HasOne(f => f.Device)
                .WithOne(d => d.FullScreenMessage)
                .HasForeignKey<FullScreenMessage>(f => f.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}