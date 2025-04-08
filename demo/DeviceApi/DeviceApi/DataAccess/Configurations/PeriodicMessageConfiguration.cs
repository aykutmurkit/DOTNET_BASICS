using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    /// <summary>
    /// PeriodicMessage entity konfigürasyonu
    /// </summary>
    public class PeriodicMessageConfiguration : IEntityTypeConfiguration<PeriodicMessage>
    {
        public void Configure(EntityTypeBuilder<PeriodicMessage> builder)
        {
            // Temel ayarlar
            builder.ToTable("PeriodicMessages");
            builder.HasKey(p => p.Id);
            
            // Id alanı yapılandırması - auto increment
            builder.Property(p => p.Id)
                  .UseIdentityColumn();
            
            // Zorunlu alanlar
            builder.Property(p => p.TemperatureLevel).IsRequired();
            builder.Property(p => p.HumidityLevel).IsRequired();
            builder.Property(p => p.GasLevel).IsRequired();
            builder.Property(p => p.FrontLightLevel).IsRequired();
            builder.Property(p => p.BackLightLevel).IsRequired();
            builder.Property(p => p.LedFailureCount).IsRequired();
            builder.Property(p => p.CabinStatus).IsRequired();
            builder.Property(p => p.FanStatus).IsRequired();
            builder.Property(p => p.ShowStatus).IsRequired();
            builder.Property(p => p.Rs232Status).IsRequired();
            builder.Property(p => p.PowerSupplyStatus).IsRequired();
            builder.Property(p => p.DeviceId).IsRequired();
            
            // Opsiyonel alanlar
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.ForecastedAt).IsRequired(false);
            
            // Device ile one-to-one ilişki
            builder.HasOne(p => p.Device)
                .WithOne(d => d.PeriodicMessage)
                .HasForeignKey<PeriodicMessage>(p => p.DeviceId)
                .OnDelete(DeleteBehavior.Cascade); // Device silinince PeriodicMessage de silinecek
        }
    }
} 